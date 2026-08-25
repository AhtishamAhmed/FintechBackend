using Application.Wrappers;
using MediatR;

namespace Application.Features.Auth.Logout
{
    public class LogoutCommand : IRequest<ApiResponse<string>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
