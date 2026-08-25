using Application.Features.Transactions.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Transactions.Admin.GetTransactions
{
    public class GetTransactionsQuery : IRequest<ApiResponse<PagedResult<TransactionDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public TransactionStatus? Status { get; set; }
        public TransactionType? Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? UserId { get; set; }
    }

    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, ApiResponse<PagedResult<TransactionDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetTransactionsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<TransactionDto>>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var query = _context.Transactions.AsQueryable();

            if (request.Status.HasValue) query = query.Where(t => t.Status == request.Status);
            if (request.Type.HasValue) query = query.Where(t => t.Type == request.Type);
            if (request.FromDate.HasValue) query = query.Where(t => t.CreatedAtUtc >= request.FromDate);
            if (request.ToDate.HasValue) query = query.Where(t => t.CreatedAtUtc <= request.ToDate);
            if (!string.IsNullOrWhiteSpace(request.UserId)) query = query.Where(t => t.InitiatedByUserId == request.UserId);

            query = query.OrderByDescending(t => t.CreatedAtUtc);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Type = t.Type.ToString(),
                    Status = t.Status.ToString(),
                    Amount = t.Amount,
                    Currency = t.Currency,
                    Description = t.Description,
                    WalletId = t.WalletId,
                    RecipientWalletId = t.RecipientWalletId,
                    FailureReason = t.FailureReason,
                    CreatedAtUtc = t.CreatedAtUtc,
                    CompletedAtUtc = t.CompletedAtUtc
                })
                .ToListAsync(cancellationToken);

            return new ApiResponse<PagedResult<TransactionDto>>(new PagedResult<TransactionDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}
