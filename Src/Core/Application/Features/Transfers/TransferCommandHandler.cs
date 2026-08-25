using Application.Exceptions;
using Application.Features.Transactions.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Transfers
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, ApiResponse<TransactionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFraudDetectionService _fraudDetectionService;
        private readonly INotificationService _notificationService;

        public TransferCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager,
            IFraudDetectionService fraudDetectionService,
            INotificationService notificationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _fraudDetectionService = fraudDetectionService;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<TransactionDto>> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            var senderId = _currentUserService.UserId!;

            if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
            {
                var existingTxn = await _context.Transactions
                    .FirstOrDefaultAsync(t => t.IdempotencyKey == request.IdempotencyKey, cancellationToken);
                if (existingTxn != null)
                {
                    return new ApiResponse<TransactionDto>(ToDto(existingTxn), "Transfer already processed (idempotent replay).");
                }
            }

            var senderWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == senderId, cancellationToken)
                ?? throw new ApiException("You do not have a wallet yet.");

            if (senderWallet.Status != WalletStatus.Active)
            {
                throw new ApiException("Your wallet is not active.");
            }

            var recipientUser = await _userManager.FindByEmailAsync(request.RecipientEmail)
                ?? throw new ApiException("Recipient not found.");

            if (recipientUser.Id == senderId)
            {
                throw new ApiException("You cannot transfer money to yourself.");
            }

            var recipientWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == recipientUser.Id, cancellationToken)
                ?? throw new ApiException("Recipient does not have a wallet.");

            if (recipientWallet.Status != WalletStatus.Active)
            {
                throw new ApiException("Recipient wallet is not active.");
            }

            if (senderWallet.Balance < request.Amount)
            {
                throw new ApiException("Insufficient wallet balance.");
            }

            var flagReason = await _fraudDetectionService.EvaluateAsync(senderId, request.Amount, cancellationToken);

            var transaction = new Transaction
            {
                Type = TransactionType.Transfer,
                Amount = request.Amount,
                Currency = senderWallet.Currency,
                Description = request.Description,
                InitiatedByUserId = senderId,
                WalletId = senderWallet.Id,
                RecipientWalletId = recipientWallet.Id,
                IdempotencyKey = request.IdempotencyKey,
                Status = flagReason != null ? TransactionStatus.Pending : TransactionStatus.Completed
            };

            if (flagReason != null)
            {
                // Money does not move yet - held for compliance review. See FraudAlert Resolve/Dismiss.
                _context.Transactions.Add(transaction);
                _context.FraudAlerts.Add(new FraudAlert { TransactionId = transaction.Id, Reason = flagReason });
                await _context.SaveChangesAsync(cancellationToken);

                await _notificationService.NotifyAsync(senderId, "Transfer under review",
                    $"Your transfer of {request.Amount:N2} {senderWallet.Currency} is under review.", cancellationToken);

                return new ApiResponse<TransactionDto>(ToDto(transaction), "Transfer is pending fraud review.");
            }

            await using var dbTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                transaction.CompletedAtUtc = DateTime.UtcNow;
                _context.Transactions.Add(transaction);

                senderWallet.Balance -= request.Amount;
                senderWallet.UpdatedAtUtc = DateTime.UtcNow;

                recipientWallet.Balance += request.Amount;
                recipientWallet.UpdatedAtUtc = DateTime.UtcNow;

                _context.TransactionEntries.Add(new TransactionEntry
                {
                    TransactionId = transaction.Id,
                    WalletId = senderWallet.Id,
                    Direction = EntryDirection.Debit,
                    Amount = request.Amount
                });

                _context.TransactionEntries.Add(new TransactionEntry
                {
                    TransactionId = transaction.Id,
                    WalletId = recipientWallet.Id,
                    Direction = EntryDirection.Credit,
                    Amount = request.Amount
                });

                await _context.SaveChangesAsync(cancellationToken);
                await dbTransaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await dbTransaction.RollbackAsync(cancellationToken);
                throw new ApiException("One of the wallets was updated concurrently. Please try again.");
            }

            await _notificationService.NotifyAsync(senderId, "Transfer sent",
                $"You sent {request.Amount:N2} {senderWallet.Currency} to {recipientUser.Email}.", cancellationToken);
            await _notificationService.NotifyAsync(recipientUser.Id, "Money received",
                $"You received {request.Amount:N2} {senderWallet.Currency} from {_currentUserService.UserId}.", cancellationToken);

            return new ApiResponse<TransactionDto>(ToDto(transaction), "Transfer completed successfully.");
        }

        private static TransactionDto ToDto(Transaction t) => new()
        {
            Id = t.Id,
            Type = t.Type.ToString(),
            Status = t.Status.ToString(),
            Amount = t.Amount,
            Currency = t.Currency,
            Description = t.Description,
            WalletId = t.WalletId,
            RecipientWalletId = t.RecipientWalletId,
            FailureReason = t.FailureReason,
            CreatedAtUtc = t.CreatedAtUtc,
            CompletedAtUtc = t.CompletedAtUtc
        };
    }
}
