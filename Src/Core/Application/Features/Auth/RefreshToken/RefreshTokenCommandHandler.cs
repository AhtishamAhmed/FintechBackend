using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<RefreshTokenResponseDto>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (existingToken == null || !existingToken.IsActive)
            {
                throw new ApiException("Invalid or expired refresh token.");
            }

            var user = await _userManager.FindByIdAsync(existingToken.UserId);
            if (user == null)
            {
                throw new ApiException("Invalid or expired refresh token.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var (accessToken, expiresAtUtc) = _tokenService.CreateAccessToken(user, roles);
            var newRefreshToken = _tokenService.CreateRefreshToken(user.Id);

            existingToken.RevokedAtUtc = DateTime.UtcNow;
            existingToken.ReplacedByToken = newRefreshToken.Token;
            await _refreshTokenRepository.UpdateAsync(existingToken);
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            var response = new RefreshTokenResponseDto
            {
                Token = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAtUtc = expiresAtUtc
            };

            return new ApiResponse<RefreshTokenResponseDto>(response, "Token refreshed successfully.");
        }
    }
}
