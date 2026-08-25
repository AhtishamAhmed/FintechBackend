using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.CurrentUser
{
    public class CurrentUserQueryHandler : IRequestHandler<CurrentUserQuery, ApiResponse<CurrentUserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public CurrentUserQueryHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<CurrentUserDto>> Handle(CurrentUserQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId == null)
            {
                throw new ApiException("Not authenticated.");
            }

            var user = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (user == null)
            {
                throw new ApiException("Not authenticated.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var response = new CurrentUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            };

            return new ApiResponse<CurrentUserDto>(response);
        }
    }
}
