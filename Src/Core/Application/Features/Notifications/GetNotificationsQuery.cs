using Application.Features.Notifications.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications
{
    public class GetNotificationsQuery : IRequest<ApiResponse<PagedResult<NotificationDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, ApiResponse<PagedResult<NotificationDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetNotificationsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<PagedResult<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var query = _context.Notifications
                .Where(n => n.UserId == _currentUserService.UserId)
                .OrderByDescending(n => n.CreatedAtUtc);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAtUtc = n.CreatedAtUtc
                })
                .ToListAsync(cancellationToken);

            return new ApiResponse<PagedResult<NotificationDto>>(new PagedResult<NotificationDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}
