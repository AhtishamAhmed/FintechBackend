using Application.Features.Wallets.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallets.Admin.GetWallets
{
    public class GetWalletsQuery : IRequest<ApiResponse<PagedResult<WalletDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetWalletsQueryHandler : IRequestHandler<GetWalletsQuery, ApiResponse<PagedResult<WalletDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetWalletsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<WalletDto>>> Handle(GetWalletsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var query = _context.Wallets.OrderByDescending(w => w.CreatedAtUtc);
            var totalCount = await query.CountAsync(cancellationToken);

            var wallets = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WalletDto
                {
                    Id = w.Id,
                    UserId = w.UserId,
                    Currency = w.Currency,
                    Balance = w.Balance,
                    Status = w.Status.ToString(),
                    CreatedAtUtc = w.CreatedAtUtc,
                    UpdatedAtUtc = w.UpdatedAtUtc
                })
                .ToListAsync(cancellationToken);

            return new ApiResponse<PagedResult<WalletDto>>(new PagedResult<WalletDto>
            {
                Items = wallets,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}
