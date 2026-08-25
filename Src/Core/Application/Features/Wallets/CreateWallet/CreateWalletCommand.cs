using Application.Features.Wallets.Common;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Wallets.CreateWallet
{
    public class CreateWalletCommand : IRequest<ApiResponse<WalletDto>>
    {
        public string Currency { get; set; } = "PKR";
    }
}
