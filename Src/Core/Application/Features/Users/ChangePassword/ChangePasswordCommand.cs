using Application.Wrappers;
using MediatR;

namespace Application.Features.Users.ChangePassword
{
    public class ChangePasswordCommand : IRequest<ApiResponse<string>>
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
