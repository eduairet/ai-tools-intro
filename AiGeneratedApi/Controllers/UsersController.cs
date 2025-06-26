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
[Route(Constants.Api.Routes.Users)]
public class UsersController(IRepositoryUsers usersRepository, IConfiguration configuration, IMapper mapper)
    : ControllerBase
{
    // POST: api/v1/users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        // Check if user with this email already exists
        var existingUsers = await usersRepository.FindAsync(u => u.Email == registerDto.Email);

        if (existingUsers.Any()) return BadRequest(Constants.Api.ErrorMessages.UserAlreadyExists);

        // Map DTO to User entity using AutoMapper
        var user = mapper.Map<User>(registerDto);
        user.PasswordHash = Helpers.Password.Hash(registerDto.Password);

        await usersRepository.AddAsync(user);
        await usersRepository.SaveChangesAsync();

        // Map User to response DTO
        var response = mapper.Map<UserResponseDto>(user);
        return Ok(response);
    }

    // POST: api/v1/users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var users = await usersRepository.FindAsync(u => u.Email == loginDto.Email);
        var user = users.FirstOrDefault();

        if (user is null)
            return Unauthorized(Constants.Api.ErrorMessages.InvalidCredentials);

        // Verify password
        if (user.PasswordHash is not null && !Helpers.Password.Verify(loginDto.Password, user.PasswordHash))
            return Unauthorized(Constants.Api.ErrorMessages.InvalidCredentials);

        // Generate JWT and refresh token
        var tokenString = Helpers.Jwt.GenerateToken(user, configuration);
        var refreshToken = Helpers.Jwt.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7).ToString(CultureInfo.InvariantCulture);
        await usersRepository.SaveChangesAsync();

        return Ok(new { token = tokenString, refreshToken });
    }

    // POST: api/v1/users/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        try
        {
            ClaimsPrincipal principal;
            try
            {
                principal = GetPrincipalFromExpiredToken(dto.AccessToken);
            }
            catch (Exception tokenEx)
            {
                return StatusCode(401,
                    new
                    {
                        error = Constants.Api.ErrorMessages.InvalidToken,
                        details = Constants.Api.ErrorMessages.TokenValidationFailed(tokenEx.Message)
                    });
            }

            var userId = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await usersRepository.GetByIdAsync(userId);
            if (user is null || user.RefreshToken != dto.RefreshToken ||
                DateTime.TryParse(user.RefreshTokenExpiryTime, out var expiryTime) && expiryTime <= DateTime.UtcNow)
                return StatusCode(401,
                    new
                    {
                        error = Constants.Api.ErrorMessages.InvalidToken,
                        details = Constants.Api.ErrorMessages.AuthenticationFailure
                    });

            var newAccessToken = Helpers.Jwt.GenerateToken(user, configuration);
            var newRefreshToken = Helpers.Jwt.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7).ToString(CultureInfo.InvariantCulture);
            await usersRepository.SaveChangesAsync();

            return Ok(new { token = newAccessToken, refreshToken = newRefreshToken });
        }
        catch (Exception ex)
        {
            return StatusCode(401,
                new { error = Constants.Api.ErrorMessages.InvalidToken, details = ex.ToString() });
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = Helpers.Jwt.GetTokenValidationParameters(configuration);
        tokenValidationParameters.ValidateLifetime = false; // Ignore expiration
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal =
            tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException(Constants.Api.ErrorMessages.InvalidToken);

        return principal;
    }
}