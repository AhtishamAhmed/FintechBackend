using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Admin.UpdateUserStatus
{
    public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, ApiResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public UpdateUserStatusCommandHandler(UserManager<ApplicationUser> userManager, INotificationService notificationService)
        {
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<string>> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new ApiException("User not found.");

            user.Status = request.Status;

            // Status alone doesn't stop Identity from signing the user in again on next login unless
            // we also engage Identity's own lockout mechanism as a second line of defense.
            if (request.Status == UserStatus.Active)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }

            await _userManager.UpdateAsync(user);
            await _notificationService.NotifyAsync(user.Id, "Account status changed", $"Your account status is now {request.Status}.", cancellationToken);

            return new ApiResponse<string>(null!, $"User status updated to {request.Status}.");
        }
    }
}
