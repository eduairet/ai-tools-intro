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
            if (user is not { Email: not null, UserName: not null })
                throw new ArgumentException("User must have Email and UserName to generate JWT token.");

            var jwtKey = configuration["Jwt:Key"] ?? Constants.Constants.Jwt.DefaultKey;
            var jwtAudience = configuration["Jwt:Audience"] ?? Constants.Constants.Jwt.DefaultAudience;
            var jwtIssuer = configuration["Jwt:Issuer"] ?? Constants.Constants.Jwt.DefaultIssuer;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Constants.Constants.Jwt.TokenExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
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
            var jwtKey = configuration["Jwt:Key"] ?? Constants.Constants.Jwt.DefaultKey;
            var jwtAudience = configuration["Jwt:Audience"] ?? Constants.Constants.Jwt.DefaultAudience;
            var jwtIssuer = configuration["Jwt:Issuer"] ?? Constants.Constants.Jwt.DefaultIssuer;

            return new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        }
    }
}