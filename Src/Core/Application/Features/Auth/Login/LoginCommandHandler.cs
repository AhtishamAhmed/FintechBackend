using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponseDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LoginCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<ApiResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new ApiException("Invalid email or password.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                throw new ApiException("Invalid email or password.");
            }

            if (user.Status != UserStatus.Active || await _userManager.IsLockedOutAsync(user))
            {
                throw new ApiException("This account is not active. Please contact support.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiresAtUtc) = _tokenService.CreateAccessToken(user, roles);

            var refreshToken = _tokenService.CreateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(refreshToken);

            var response = new LoginResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Roles = roles.ToList(),
                Token = token,
                RefreshToken = refreshToken.Token,
                ExpiresAtUtc = expiresAtUtc
            };

            return new ApiResponse<LoginResponseDto>(response, "Login successful.");
        }
    }
}
