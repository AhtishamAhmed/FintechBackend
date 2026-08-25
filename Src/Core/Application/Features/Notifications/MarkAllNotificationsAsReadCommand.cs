using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications
{
    public class MarkAllNotificationsAsReadCommand : IRequest<ApiResponse<string>>
    {
    }

    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public MarkAllNotificationsAsReadCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<string>> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserId == _currentUserService.UserId && !n.IsRead)
                .ToListAsync(cancellationToken);

            foreach (var n in unread)
            {
                n.IsRead = true;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<string>(null!, $"{unread.Count} notification(s) marked as read.");
        }
    }
}
