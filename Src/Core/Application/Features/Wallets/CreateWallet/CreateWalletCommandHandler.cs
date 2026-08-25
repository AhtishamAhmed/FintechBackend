using Application.Exceptions;
using Application.Features.Wallets.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallets.CreateWallet
{
    public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, ApiResponse<WalletDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateWalletCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<WalletDto>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!;

            var existing = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
            if (existing != null)
            {
                throw new ApiException("You already have a wallet.");
            }

            var wallet = new Wallet
            {
                UserId = userId,
                Currency = request.Currency,
                Balance = 0
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<WalletDto>(new WalletDto
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                Currency = wallet.Currency,
                Balance = wallet.Balance,
                Status = wallet.Status.ToString(),
                CreatedAtUtc = wallet.CreatedAtUtc,
                UpdatedAtUtc = wallet.UpdatedAtUtc
            }, "Wallet created successfully.");
        }
    }
}
