using Application.Features.Fraud.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reports
{
    public class GetFraudReportQuery : IRequest<ApiResponse<PagedResult<FraudAlertDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public FraudAlertStatus? Status { get; set; }
    }

    public class GetFraudReportQueryHandler : IRequestHandler<GetFraudReportQuery, ApiResponse<PagedResult<FraudAlertDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetFraudReportQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<FraudAlertDto>>> Handle(GetFraudReportQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var query = _context.FraudAlerts.AsQueryable();
            if (request.Status.HasValue) query = query.Where(f => f.Status == request.Status);
            query = query.OrderByDescending(f => f.CreatedAtUtc);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FraudAlertDto
                {
                    Id = f.Id,
                    TransactionId = f.TransactionId,
                    Reason = f.Reason,
                    Status = f.Status.ToString(),
                    ResolutionNotes = f.ResolutionNotes,
                    CreatedAtUtc = f.CreatedAtUtc,
                    ReviewedAtUtc = f.ReviewedAtUtc
                })
                .ToListAsync(cancellationToken);

            return new ApiResponse<PagedResult<FraudAlertDto>>(new PagedResult<FraudAlertDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}
