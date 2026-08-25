using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallets.Admin.UpdateWalletStatus
{
    public class UpdateWalletStatusCommandHandler : IRequestHandler<UpdateWalletStatusCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public UpdateWalletStatusCommandHandler(IApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<string>> Handle(UpdateWalletStatusCommand request, CancellationToken cancellationToken)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == request.WalletId, cancellationToken)
                ?? throw new ApiException("Wallet not found.");

            wallet.Status = request.Status;
            wallet.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyAsync(wallet.UserId, "Wallet status changed", $"Your wallet status is now {request.Status}.", cancellationToken);

            return new ApiResponse<string>(null!, $"Wallet status updated to {request.Status}.");
        }
    }
}
