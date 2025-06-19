using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventManagementApi.Models.User;
using EventManagementApi.Shared.Constants;

namespace EventManagementApi.Shared.Helpers
{
    public static partial class Helpers
    {
        public static class Jwt
        {
            public static string GenerateToken(User user, IConfiguration configuration)
            {
                var jwtKey = configuration["Jwt:Key"] ?? EventManagementApi.Shared.Constants.Jwt.DefaultSecretKey;
                var jwtIssuer = configuration["Jwt:Issuer"] ?? EventManagementApi.Shared.Constants.Jwt.DefaultIssuer;
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: null,
                    claims: claims,
                    expires: DateTime.Now.AddHours(EventManagementApi.Shared.Constants.Jwt.TokenExpirationHours),
                    signingCredentials: creds
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
            {
                var jwtKey = configuration["Jwt:Key"] ?? EventManagementApi.Shared.Constants.Jwt.DefaultSecretKey;
                var jwtIssuer = configuration["Jwt:Issuer"] ?? EventManagementApi.Shared.Constants.Jwt.DefaultIssuer;
                return new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            }
        }
    }
} 