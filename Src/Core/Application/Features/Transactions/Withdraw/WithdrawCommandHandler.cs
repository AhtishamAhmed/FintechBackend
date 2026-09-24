using Application.Exceptions;
using Application.Features.Transactions.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Transactions.Withdraw
{
    public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, ApiResponse<TransactionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFraudDetectionService _fraudDetectionService;
        private readonly INotificationService _notificationService;

        public WithdrawCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IFraudDetectionService fraudDetectionService,
            INotificationService notificationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _fraudDetectionService = fraudDetectionService;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<TransactionDto>> Handle(WithdrawCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!;

            if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
            {
                var existingTxn = await _context.Transactions
                    .FirstOrDefaultAsync(t => t.IdempotencyKey == request.IdempotencyKey, cancellationToken);
                if (existingTxn != null)
                {
                    return new ApiResponse<TransactionDto>(ToDto(existingTxn), "Withdrawal already processed (idempotent replay).");
                }
            }

            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken)
                ?? throw new ApiException("You do not have a wallet yet.");

            if (wallet.Status != WalletStatus.Active)
            {
                throw new ApiException("Your wallet is not active.");
            }

            if (wallet.Balance < request.Amount)
            {
                throw new ApiException("Insufficient wallet balance.");
            }

            var kyc = await _context.KycApplications.FirstOrDefaultAsync(k => k.UserId == userId, cancellationToken);
            if (kyc == null || kyc.Status != KycStatus.Approved)
            {
                throw new ApiException("You must complete KYC verification before making transactions.");
            }

            var flagReason = await _fraudDetectionService.EvaluateAsync(userId, request.Amount, cancellationToken);

            var transaction = new Transaction
            {
                Type = TransactionType.Withdrawal,
                Amount = request.Amount,
                Currency = wallet.Currency,
                Description = request.Description,
                InitiatedByUserId = userId,
                WalletId = wallet.Id,
                IdempotencyKey = request.IdempotencyKey,
                Status = flagReason != null ? TransactionStatus.Pending : TransactionStatus.Completed
            };

            if (flagReason != null)
            {
                _context.Transactions.Add(transaction);
                _context.FraudAlerts.Add(new FraudAlert { TransactionId = transaction.Id, Reason = flagReason });
                await _context.SaveChangesAsync(cancellationToken);

                await _notificationService.NotifyAsync(userId, "Withdrawal under review",
                    $"Your withdrawal of {request.Amount:N2} {wallet.Currency} is under review.", cancellationToken);

                return new ApiResponse<TransactionDto>(ToDto(transaction), "Withdrawal is pending fraud review.");
            }

            await using var dbTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                transaction.CompletedAtUtc = DateTime.UtcNow;
                _context.Transactions.Add(transaction);

                wallet.Balance -= request.Amount;
                wallet.UpdatedAtUtc = DateTime.UtcNow;

                _context.TransactionEntries.Add(new TransactionEntry
                {
                    TransactionId = transaction.Id,
                    WalletId = wallet.Id,
                    Direction = EntryDirection.Debit,
                    Amount = request.Amount
                });

                await _context.SaveChangesAsync(cancellationToken);
                await dbTransaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await dbTransaction.RollbackAsync(cancellationToken);
                throw new ApiException("Your wallet was updated concurrently. Please try again.");
            }

            await _notificationService.NotifyAsync(userId, "Withdrawal successful",
                $"Your withdrawal of {request.Amount:N2} {wallet.Currency} was successful.", cancellationToken);

            return new ApiResponse<TransactionDto>(ToDto(transaction), "Withdrawal completed successfully.");
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
