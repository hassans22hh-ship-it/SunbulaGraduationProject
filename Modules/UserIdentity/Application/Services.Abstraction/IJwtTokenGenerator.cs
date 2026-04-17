using Domain.Entities;

namespace Application.Services.Abstraction
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        int GetRefreshTokenExpiryDays();
    }
}
