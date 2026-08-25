using Application.Wrappers;
using MediatR;

namespace Application.Features.Auth.ResetPassword
{
    public class ResetPasswordCommand : IRequest<ApiResponse<string>>
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
