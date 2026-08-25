using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IApplicationDbContext _context;

        public NotificationService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task NotifyAsync(string userId, string title, string message, CancellationToken cancellationToken = default)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = userId,
                Title = title,
                Message = message
            });

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
