using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications
{
    public class MarkNotificationAsReadCommand : IRequest<ApiResponse<string>>
    {
        public Guid NotificationId { get; set; }
    }

    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public MarkNotificationAsReadCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<string>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var n = await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == request.NotificationId && x.UserId == _currentUserService.UserId, cancellationToken)
                ?? throw new ApiException("Notification not found.");

            n.IsRead = true;
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<string>(null!, "Notification marked as read.");
        }
    }
}
