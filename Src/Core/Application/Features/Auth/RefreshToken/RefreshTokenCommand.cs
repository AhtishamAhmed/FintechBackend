using Application.Wrappers;
using MediatR;

namespace Application.Features.Auth.RefreshToken
{
    public class RefreshTokenCommand : IRequest<ApiResponse<RefreshTokenResponseDto>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
