using Application.Wrappers;
using MediatR;

namespace Application.Features.Auth.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<ApiResponse<string>>
    {
        public string Email { get; set; } = string.Empty;
    }
}
