using Application.Exceptions;
using Application.Features.Notifications.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications
{
    public class GetNotificationByIdQuery : IRequest<ApiResponse<NotificationDto>>
    {
        public Guid NotificationId { get; set; }
    }

    public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, ApiResponse<NotificationDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetNotificationByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<NotificationDto>> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
        {
            var n = await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == request.NotificationId && x.UserId == _currentUserService.UserId, cancellationToken)
                ?? throw new ApiException("Notification not found.");

            return new ApiResponse<NotificationDto>(new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAtUtc = n.CreatedAtUtc
            });
        }
    }
}
