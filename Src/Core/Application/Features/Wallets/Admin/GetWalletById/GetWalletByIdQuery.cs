using Application.Exceptions;
using Application.Features.Wallets.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallets.Admin.GetWalletById
{
    public class GetWalletByIdQuery : IRequest<ApiResponse<WalletDto>>
    {
        public Guid WalletId { get; set; }
    }

    public class GetWalletByIdQueryHandler : IRequestHandler<GetWalletByIdQuery, ApiResponse<WalletDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetWalletByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<WalletDto>> Handle(GetWalletByIdQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == request.WalletId, cancellationToken)
                ?? throw new ApiException("Wallet not found.");

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
