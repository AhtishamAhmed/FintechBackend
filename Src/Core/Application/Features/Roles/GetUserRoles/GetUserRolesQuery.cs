using Application.Exceptions;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Roles.GetUserRoles
{
    public class GetUserRolesQuery : IRequest<ApiResponse<List<string>>>
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, ApiResponse<List<string>>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserRolesQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResponse<List<string>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new ApiException("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            return new ApiResponse<List<string>>(roles.ToList());
        }
    }
}
