using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Fraud
{
    public class ResolveFraudAlertCommand : IRequest<ApiResponse<string>>
    {
        public Guid AlertId { get; set; }
        public string? Notes { get; set; }
    }

    public class ResolveFraudAlertCommandHandler : IRequestHandler<ResolveFraudAlertCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        public ResolveFraudAlertCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, INotificationService notificationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<string>> Handle(ResolveFraudAlertCommand request, CancellationToken cancellationToken)
        {
            var alert = await _context.FraudAlerts.FirstOrDefaultAsync(f => f.Id == request.AlertId, cancellationToken)
                ?? throw new ApiException("Fraud alert not found.");

            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == alert.TransactionId, cancellationToken)
                ?? throw new ApiException("Linked transaction not found.");

            // Resolve = confirmed legitimate. If money hasn't moved yet (still Pending), move it now.
            if (transaction.Status == TransactionStatus.Pending)
            {
                await using var dbTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var senderWallet = await _context.Wallets.FirstAsync(w => w.Id == transaction.WalletId, cancellationToken);

                    if (transaction.Type == TransactionType.Transfer && transaction.RecipientWalletId.HasValue)
                    {
                        var recipientWallet = await _context.Wallets.FirstAsync(w => w.Id == transaction.RecipientWalletId.Value, cancellationToken);

                        senderWallet.Balance -= transaction.Amount;
                        senderWallet.UpdatedAtUtc = DateTime.UtcNow;
                        recipientWallet.Balance += transaction.Amount;
                        recipientWallet.UpdatedAtUtc = DateTime.UtcNow;

                        _context.TransactionEntries.Add(new TransactionEntry { TransactionId = transaction.Id, WalletId = senderWallet.Id, Direction = EntryDirection.Debit, Amount = transaction.Amount });
                        _context.TransactionEntries.Add(new TransactionEntry { TransactionId = transaction.Id, WalletId = recipientWallet.Id, Direction = EntryDirection.Credit, Amount = transaction.Amount });
                    }
                    else if (transaction.Type == TransactionType.Withdrawal)
                    {
                        senderWallet.Balance -= transaction.Amount;
                        senderWallet.UpdatedAtUtc = DateTime.UtcNow;
                        _context.TransactionEntries.Add(new TransactionEntry { TransactionId = transaction.Id, WalletId = senderWallet.Id, Direction = EntryDirection.Debit, Amount = transaction.Amount });
                    }
                    else
                    {
                        senderWallet.Balance += transaction.Amount;
                        senderWallet.UpdatedAtUtc = DateTime.UtcNow;
                        _context.TransactionEntries.Add(new TransactionEntry { TransactionId = transaction.Id, WalletId = senderWallet.Id, Direction = EntryDirection.Credit, Amount = transaction.Amount });
                    }

                    transaction.Status = TransactionStatus.Completed;
                    transaction.CompletedAtUtc = DateTime.UtcNow;

                    await _context.SaveChangesAsync(cancellationToken);
                    await dbTransaction.CommitAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    await dbTransaction.RollbackAsync(cancellationToken);
                    throw new ApiException("A wallet involved was updated concurrently. Please try again.");
                }
            }

            alert.Status = FraudAlertStatus.Resolved;
            alert.ReviewedByUserId = _currentUserService.UserId;
            alert.ReviewedAtUtc = DateTime.UtcNow;
            alert.ResolutionNotes = request.Notes;
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyAsync(transaction.InitiatedByUserId, "Transaction cleared",
                $"Your {transaction.Type} of {transaction.Amount:N2} {transaction.Currency} has been cleared and processed.", cancellationToken);

            return new ApiResponse<string>(null!, "Fraud alert resolved and transaction processed.");
        }
    }
}
