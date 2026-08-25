using Application.Exceptions;
using Application.Features.Users.Common;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Admin.GetUserById
{
    public class GetUserByIdQuery : IRequest<ApiResponse<UserProfileDto>>
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ApiResponse<UserProfileDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResponse<UserProfileDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new ApiException("User not found.");

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
