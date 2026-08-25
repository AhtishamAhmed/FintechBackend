using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Auth.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponse<string>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICurrentUserService _currentUserService;

        public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository, ICurrentUserService currentUserService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (existingToken == null || existingToken.UserId != _currentUserService.UserId)
            {
                throw new ApiException("Invalid refresh token.");
            }

            if (existingToken.IsActive)
            {
                existingToken.RevokedAtUtc = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(existingToken);
            }

            return new ApiResponse<string>(null!, "Logged out successfully.");
        }
    }
}
