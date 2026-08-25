using Application.Wrappers;
using MediatR;

namespace Application.Features.Roles.UpdateUserRoles
{
    public class UpdateUserRolesCommand : IRequest<ApiResponse<List<string>>>
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
