using Application.Features.AuditLogs.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AuditLogs
{
    public class GetAuditLogsQuery : IRequest<ApiResponse<PagedResult<AuditLogDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? UserId { get; set; }
        public string? Action { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, ApiResponse<PagedResult<AuditLogDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAuditLogsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var query = _context.AuditLogs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.UserId)) query = query.Where(a => a.UserId == request.UserId);
            if (!string.IsNullOrWhiteSpace(request.Action)) query = query.Where(a => a.Action == request.Action);
            if (request.FromDate.HasValue) query = query.Where(a => a.TimestampUtc >= request.FromDate);
            if (request.ToDate.HasValue) query = query.Where(a => a.TimestampUtc <= request.ToDate);

            query = query.OrderByDescending(a => a.TimestampUtc);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    Action = a.Action,
                    EntityType = a.EntityType,
                    EntityId = a.EntityId,
                    TimestampUtc = a.TimestampUtc
                })
                .ToListAsync(cancellationToken);

            return new ApiResponse<PagedResult<AuditLogDto>>(new PagedResult<AuditLogDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}
