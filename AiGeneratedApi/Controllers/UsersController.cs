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

namespace EventManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            user.Id = Guid.NewGuid().ToString();
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

            // Generate JWT using helper method
            var tokenString = Helpers.Jwt.GenerateToken(user, _configuration);
            return Ok(new { token = tokenString });
        }

        // POST: api/users/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshDto)
        {
            try
            {
                // Validate the current token
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = Helpers.Jwt.GetTokenValidationParameters(_configuration);

                // Validate and read the token
                var principal = tokenHandler.ValidateToken(refreshDto.Token, validationParameters, out var validatedToken);
                
                // Extract user ID from the token
                var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(Constants.ApiConstants.ErrorMessages.InvalidToken);
                }

                // Verify user still exists
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(Constants.ApiConstants.ErrorMessages.UserNotFound);
                }

                // Generate new token
                var newToken = Helpers.Jwt.GenerateToken(user, _configuration);
                
                return Ok(new { token = newToken });
            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized(Constants.ApiConstants.ErrorMessages.TokenExpired);
            }
            catch (Exception)
            {
                return Unauthorized(Constants.ApiConstants.ErrorMessages.InvalidToken);
            }
        }
    }
}
