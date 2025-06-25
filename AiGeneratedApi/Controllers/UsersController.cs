using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using EventManagementApi.Models;
using EventManagementApi.Models.Dto.User;
using EventManagementApi.Repositories.RepositoryUsers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using EventManagementApi.Shared.Constants;
using EventManagementApi.Shared.Helpers;

namespace EventManagementApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IRepositoryUsers userRepository, IConfiguration configuration, IMapper mapper)
    : ControllerBase
{
    // POST: api/users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        // Check if user with this email already exists
        var existingUsers = await userRepository.FindAsync(u => u.Email == registerDto.Email);

        if (existingUsers.Any()) return BadRequest(Constants.ApiConstants.ErrorMessages.UserAlreadyExists);

        // Map DTO to User entity using AutoMapper
        var user = mapper.Map<User>(registerDto);
        user.PasswordHash = Helpers.Password.Hash(registerDto.Password);

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();

        // Map User to response DTO
        var response = mapper.Map<UserResponseDto>(user);
        return Ok(response);
    }

    // POST: api/users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var users = await userRepository.FindAsync(u => u.Email == loginDto.Email);
        var user = users.FirstOrDefault();

        if (user is null)
            return Unauthorized(Constants.ApiConstants.ErrorMessages.InvalidCredentials);

        // Verify password
        if (user.PasswordHash is not null && !Helpers.Password.Verify(loginDto.Password, user.PasswordHash))
            return Unauthorized(Constants.ApiConstants.ErrorMessages.InvalidCredentials);

        // Generate JWT and refresh token
        var tokenString = Helpers.Jwt.GenerateToken(user, configuration);
        var refreshToken = Helpers.Jwt.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7).ToString(CultureInfo.InvariantCulture);
        await userRepository.SaveChangesAsync();

        return Ok(new { token = tokenString, refreshToken });
    }

    // POST: api/users/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        Console.WriteLine(
            $"[RefreshToken] Called with accessToken: {dto.AccessToken.Substring(0, 20)}..., refreshToken: {dto.RefreshToken.Substring(0, 8)}...");
        try
        {
            ClaimsPrincipal principal;
            try
            {
                principal = GetPrincipalFromExpiredToken(dto.AccessToken);
            }
            catch (Exception tokenEx)
            {
                Console.WriteLine($"[RefreshToken TokenException] {tokenEx}");
                return StatusCode(401,
                    new
                    {
                        error = Constants.ApiConstants.ErrorMessages.InvalidToken,
                        details = "Token validation failed: " + tokenEx
                    });
            }

            var userId = principal.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
            var user = await userRepository.GetByIdAsync(userId);
            if (user is null || user.RefreshToken != dto.RefreshToken ||
                DateTime.TryParse(user.RefreshTokenExpiryTime, out var expiryTime) && expiryTime <= DateTime.UtcNow)
                return StatusCode(401,
                    new
                    {
                        error = Constants.ApiConstants.ErrorMessages.InvalidToken,
                        details = "User not found, refresh token mismatch, or refresh token expired."
                    });

            var newAccessToken = Helpers.Jwt.GenerateToken(user, configuration);
            var newRefreshToken = Helpers.Jwt.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7).ToString(CultureInfo.InvariantCulture);
            await userRepository.SaveChangesAsync();

            return Ok(new { token = newAccessToken, refreshToken = newRefreshToken });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RefreshToken Exception] {ex}");
            return StatusCode(401,
                new { error = Constants.ApiConstants.ErrorMessages.InvalidToken, details = ex.ToString() });
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = Helpers.Jwt.GetTokenValidationParameters(configuration);
        tokenValidationParameters.ValidateLifetime = false; // Ignore expiration
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal =
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }
}