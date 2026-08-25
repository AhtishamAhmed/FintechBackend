using Application.Features.Transactions.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reports
{
    public class GetTransactionsReportQuery : IRequest<ApiResponse<PagedResult<TransactionDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TransactionStatus? Status { get; set; }
        public TransactionType? TransactionType { get; set; }
        public string? UserId { get; set; }
        public string? Currency { get; set; }
    }

    public class GetTransactionsReportQueryHandler : IRequestHandler<GetTransactionsReportQuery, ApiResponse<PagedResult<TransactionDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetTransactionsReportQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<TransactionDto>>> Handle(GetTransactionsReportQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var query = _context.Transactions.AsQueryable();
            if (request.FromDate.HasValue) query = query.Where(t => t.CreatedAtUtc >= request.FromDate);
            if (request.ToDate.HasValue) query = query.Where(t => t.CreatedAtUtc <= request.ToDate);
            if (request.Status.HasValue) query = query.Where(t => t.Status == request.Status);
            if (request.TransactionType.HasValue) query = query.Where(t => t.Type == request.TransactionType);
            if (!string.IsNullOrWhiteSpace(request.UserId)) query = query.Where(t => t.InitiatedByUserId == request.UserId);
            if (!string.IsNullOrWhiteSpace(request.Currency)) query = query.Where(t => t.Currency == request.Currency);

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
