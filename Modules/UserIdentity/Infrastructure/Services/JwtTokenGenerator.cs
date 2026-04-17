using Application.Options;
using Application.Services.Abstraction;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace UserIdentityInfrastructure.Services
{
    public sealed class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions;
        public JwtTokenGenerator(IOptions<JwtOptions>jwtOption)
        {
            _jwtOptions = jwtOption.Value;
        }
        public string GenerateAccessToken(User user)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {

            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public int GetRefreshTokenExpiryDays()
        {
            return _jwtOptions.RefreshTokenExpiryDays;
        }
        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private static List<Claim> GetClaims(User user)
        {
            return new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email.Value),
            new(ClaimTypes.Name, user.GetFullName()),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new("IsEmailConfirmed", user.IsEmailConfirmed.ToString()),
            new("IsActive", user.IsActive.ToString()),
            new("CoinBalance", user.CoinBalance.ToString()),
            new(ClaimTypes.Role, user.Role.ToString())
        };
        }
    }
}
