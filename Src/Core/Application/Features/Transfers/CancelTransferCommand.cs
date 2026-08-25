using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Transfers
{
    public class CancelTransferCommand : IRequest<ApiResponse<string>>
    {
        public Guid TransferId { get; set; }
    }

    public class CancelTransferCommandHandler : IRequestHandler<CancelTransferCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CancelTransferCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<string>> Handle(CancelTransferCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == request.TransferId && t.Type == TransactionType.Transfer, cancellationToken)
                ?? throw new ApiException("Transfer not found.");

            if (transaction.InitiatedByUserId != _currentUserService.UserId)
            {
                throw new ApiException("Transfer not found.");
            }

            if (transaction.Status != TransactionStatus.Pending)
            {
                throw new ApiException("Only a transfer that is pending review can be cancelled.");
            }

            transaction.Status = TransactionStatus.Cancelled;

            var alert = await _context.FraudAlerts.FirstOrDefaultAsync(f => f.TransactionId == transaction.Id, cancellationToken);
            if (alert != null)
            {
                alert.Status = Domain.Enums.FraudAlertStatus.Dismissed;
                alert.ReviewedAtUtc = DateTime.UtcNow;
                alert.ResolutionNotes = "Cancelled by customer.";
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<string>(null!, "Transfer cancelled.");
        }
    }
}
