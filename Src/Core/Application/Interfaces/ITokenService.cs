using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAtUtc) CreateAccessToken(ApplicationUser user, IList<string> roles);
        RefreshToken CreateRefreshToken(string userId);
    }
}
