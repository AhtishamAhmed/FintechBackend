using Application.Features.Users.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.GetProfile
{
    public class GetProfileQuery : IRequest<ApiResponse<UserProfileDto>>
    {
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ApiResponse<UserProfileDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public GetProfileQueryHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<UserProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId!)
                ?? throw new Exceptions.ApiException("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            return new ApiResponse<UserProfileDto>(new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status.ToString(),
                Roles = roles.ToList()
            });
        }
    }
}
