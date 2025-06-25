using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventManagementApi.Models;
using System.Security.Cryptography;

namespace EventManagementApi.Shared.Helpers;

public static partial class Helpers
{
    public static class Jwt
    {
        public static string GenerateToken(User user, IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"] ?? Constants.Jwt.DefaultKey;
            var jwtIssuer = configuration["Jwt:Issuer"] ?? Constants.Jwt.DefaultIssuer;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Constants.Jwt.TokenExpirationHours),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"] ?? Constants.Jwt.DefaultKey;
            var jwtIssuer = configuration["Jwt:Issuer"] ?? Constants.Jwt.DefaultIssuer;

            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false, // No audience specified in token generation
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero // No tolerance for expiration
            };
        }
    }
}