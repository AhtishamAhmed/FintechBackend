using Application.Features.Support.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Support
{
    public class GetSupportTicketsQuery : IRequest<ApiResponse<PagedResult<SupportTicketDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetSupportTicketsQueryHandler : IRequestHandler<GetSupportTicketsQuery, ApiResponse<PagedResult<SupportTicketDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetSupportTicketsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<PagedResult<SupportTicketDto>>> Handle(GetSupportTicketsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var isStaff = _currentUserService.IsInRole("SupportAgent") || _currentUserService.IsInRole("Admin");

            var query = isStaff
                ? _context.SupportTickets.AsQueryable()
                : _context.SupportTickets.Where(t => t.CustomerUserId == _currentUserService.UserId);

            query = query.OrderByDescending(t => t.CreatedAtUtc);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new SupportTicketDto
                {
                    Id = t.Id,
                    CustomerUserId = t.CustomerUserId,
                    Subject = t.Subject,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    AssignedAgentUserId = t.AssignedAgentUserId,
                    CreatedAtUtc = t.CreatedAtUtc,
                    UpdatedAtUtc = t.UpdatedAtUtc,
                    ClosedAtUtc = t.ClosedAtUtc
                })
                .ToListAsync(cancellationToken);

            return new ApiResponse<PagedResult<SupportTicketDto>>(new PagedResult<SupportTicketDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}
