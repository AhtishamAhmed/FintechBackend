using Application.Wrappers;
using Domain.Enums;
using MediatR;

namespace Application.Features.Users.Admin.UpdateUserStatus
{
    public class UpdateUserStatusCommand : IRequest<ApiResponse<string>>
    {
        public string UserId { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
    }
}
