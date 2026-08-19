using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAtUtc) CreateToken(ApplicationUser user, IList<string> roles);
    }
}
