using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EventManagementApi.Models.User;
using EventManagementApi.Repositories.RepositoryUsers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System;
using AutoMapper;
using EventManagementApi.Shared.Constants;
using EventManagementApi.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace EventManagementApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IRepositoryUsers _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UsersController(IRepositoryUsers userRepository, IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Check if user with this email already exists
            var existingUsers = await _userRepository.FindAsync(u => u.Email == registerDto.Email);
            if (existingUsers.Any())
            {
                return BadRequest(Constants.ApiConstants.ErrorMessages.UserAlreadyExists);
            }

            // Map DTO to User entity using AutoMapper
            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = Helpers.Password.Hash(registerDto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Map User to response DTO
            var response = _mapper.Map<UserResponseDto>(user);
            return Ok(response);
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var users = await _userRepository.FindAsync(u => u.Email == loginDto.Email);
            var user = users.FirstOrDefault();
            if (user == null)
                return Unauthorized(Constants.ApiConstants.ErrorMessages.InvalidCredentials);

            // Verify password
            if (!Helpers.Password.Verify(loginDto.Password, user.PasswordHash ?? string.Empty))
                return Unauthorized(Constants.ApiConstants.ErrorMessages.InvalidCredentials);

            // Generate JWT and refresh token
            var tokenString = Helpers.Jwt.GenerateToken(user, _configuration);
            var refreshToken = Helpers.Jwt.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.SaveChangesAsync();

            return Ok(new { token = tokenString, refreshToken });
        }

        // POST: api/users/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            Console.WriteLine($"[RefreshToken] Called with accessToken: {dto.AccessToken.Substring(0, 20)}..., refreshToken: {dto.RefreshToken.Substring(0, 8)}...");
            ClaimsPrincipal principal = null;
            try
            {
                try
                {
                    principal = GetPrincipalFromExpiredToken(dto.AccessToken);
                }
                catch (Exception tokenEx)
                {
                    Console.WriteLine($"[RefreshToken TokenException] {tokenEx}");
                    return StatusCode(401, new { error = Constants.ApiConstants.ErrorMessages.InvalidToken, details = "Token validation failed: " + tokenEx.ToString() });
                }
                var userId = principal.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    return StatusCode(401, new { error = Constants.ApiConstants.ErrorMessages.InvalidToken, details = "User not found, refresh token mismatch, or refresh token expired." });

                var newAccessToken = Helpers.Jwt.GenerateToken(user, _configuration);
                var newRefreshToken = Helpers.Jwt.GenerateRefreshToken();
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userRepository.SaveChangesAsync();

                return Ok(new { token = newAccessToken, refreshToken = newRefreshToken });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RefreshToken Exception] {ex}");
                return StatusCode(401, new { error = Constants.ApiConstants.ErrorMessages.InvalidToken, details = ex.ToString() });
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = Helpers.Jwt.GetTokenValidationParameters(_configuration);
            tokenValidationParameters.ValidateLifetime = false; // Ignore expiration
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}
