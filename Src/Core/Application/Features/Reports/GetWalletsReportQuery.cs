using Application.Features.Wallets.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reports
{
    public class GetWalletsReportQuery : IRequest<ApiResponse<PagedResult<WalletDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public WalletStatus? Status { get; set; }
        public string? Currency { get; set; }
    }

    public class GetWalletsReportQueryHandler : IRequestHandler<GetWalletsReportQuery, ApiResponse<PagedResult<WalletDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetWalletsReportQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<WalletDto>>> Handle(GetWalletsReportQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var query = _context.Wallets.AsQueryable();
            if (request.Status.HasValue) query = query.Where(w => w.Status == request.Status);
            if (!string.IsNullOrWhiteSpace(request.Currency)) query = query.Where(w => w.Currency == request.Currency);

            query = query.OrderByDescending(w => w.CreatedAtUtc);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
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
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}
