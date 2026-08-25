using Application.Wrappers;
using Domain.Enums;
using MediatR;

namespace Application.Features.Wallets.Admin.UpdateWalletStatus
{
    public class UpdateWalletStatusCommand : IRequest<ApiResponse<string>>
    {
        public Guid WalletId { get; set; }
        public WalletStatus Status { get; set; }
    }
}
