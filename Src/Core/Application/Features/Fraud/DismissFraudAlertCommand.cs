using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Fraud
{
    public class DismissFraudAlertCommand : IRequest<ApiResponse<string>>
    {
        public Guid AlertId { get; set; }
        public string? Notes { get; set; }
    }

    public class DismissFraudAlertCommandHandler : IRequestHandler<DismissFraudAlertCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        public DismissFraudAlertCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, INotificationService notificationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<string>> Handle(DismissFraudAlertCommand request, CancellationToken cancellationToken)
        {
            var alert = await _context.FraudAlerts.FirstOrDefaultAsync(f => f.Id == request.AlertId, cancellationToken)
                ?? throw new ApiException("Fraud alert not found.");

            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == alert.TransactionId, cancellationToken)
                ?? throw new ApiException("Linked transaction not found.");

            // Dismiss = confirmed fraudulent / rejected. Money never moved (it was held pending review), so
            // there is nothing to reverse - we simply cancel the transaction.
            if (transaction.Status == TransactionStatus.Pending)
            {
                transaction.Status = TransactionStatus.Rejected;
            }

            alert.Status = FraudAlertStatus.Dismissed;
            alert.ReviewedByUserId = _currentUserService.UserId;
            alert.ReviewedAtUtc = DateTime.UtcNow;
            alert.ResolutionNotes = request.Notes;
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyAsync(transaction.InitiatedByUserId, "Transaction rejected",
                $"Your {transaction.Type} of {transaction.Amount:N2} {transaction.Currency} was rejected after review.", cancellationToken);

            return new ApiResponse<string>(null!, "Fraud alert dismissed and transaction rejected.");
        }
    }
}
