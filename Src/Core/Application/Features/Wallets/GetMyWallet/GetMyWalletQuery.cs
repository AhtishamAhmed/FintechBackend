using Application.Exceptions;
using Application.Features.Wallets.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallets.GetMyWallet
{
    public class GetMyWalletQuery : IRequest<ApiResponse<WalletDto>>
    {
    }

    public class GetMyWalletQueryHandler : IRequestHandler<GetMyWalletQuery, ApiResponse<WalletDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetMyWalletQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<WalletDto>> Handle(GetMyWalletQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == _currentUserService.UserId, cancellationToken)
                ?? throw new ApiException("You do not have a wallet yet.");

            return new ApiResponse<WalletDto>(new WalletDto
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                Currency = wallet.Currency,
                Balance = wallet.Balance,
                Status = wallet.Status.ToString(),
                CreatedAtUtc = wallet.CreatedAtUtc,
                UpdatedAtUtc = wallet.UpdatedAtUtc
            });
        }
    }
}
