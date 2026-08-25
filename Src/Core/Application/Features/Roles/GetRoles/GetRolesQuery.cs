using Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Roles.GetRoles
{
    public class GetRolesQuery : IRequest<ApiResponse<List<string>>>
    {
    }

    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, ApiResponse<List<string>>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetRolesQueryHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public Task<ApiResponse<List<string>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = _roleManager.Roles.Select(r => r.Name!).ToList();
            return Task.FromResult(new ApiResponse<List<string>>(roles));
        }
    }
}
