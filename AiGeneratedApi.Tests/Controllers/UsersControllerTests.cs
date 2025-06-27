using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EventManagementApi.Controllers;
using EventManagementApi.Models;
using EventManagementApi.Models.Dto.User;
using EventManagementApi.Repositories.RepositoryUsers;
using EventManagementApi.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
namespace AiGeneratedApi.Tests.Controllers;

public class UsersControllerTests : TestBase
{
    private readonly UsersController _controller;
    private readonly Mock<IRepositoryUsers> _usersRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IConfigurationSection> _configSectionMock;

    public UsersControllerTests()
    {
        _usersRepositoryMock = new Mock<IRepositoryUsers>();
        _configurationMock = new Mock<IConfiguration>();
        _configSectionMock = new Mock<IConfigurationSection>();

        // Setup configuration mock for JWT settings
        _configSectionMock.Setup(s => s.Value).Returns("testSecretKeyWithMinimum32Characters1234567890");
        _configurationMock.Setup(c => c.GetSection("JwtSettings:Secret")).Returns(_configSectionMock.Object);
        _configurationMock.Setup(c => c.GetSection("JwtSettings:Issuer")).Returns(_configSectionMock.Object);
        _configurationMock.Setup(c => c.GetSection("JwtSettings:Audience")).Returns(_configSectionMock.Object);

        _controller = new UsersController(
            _usersRepositoryMock.Object,
            _configurationMock.Object,
            Mapper);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Test123!"
        };

        _usersRepositoryMock.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        _usersRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _usersRepositoryMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UserResponseDto>(okResult.Value);
        Assert.Equal(registerDto.Email, response.Email);
        // Password should not be returned in the response
        _usersRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
        _usersRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "existing@example.com",
            Password = "Test123!"
        };

        var existingUser = new User
        {
            Id = "existingUserId",
            UserName = "existingUser",
            Email = registerDto.Email
        };

        _usersRepositoryMock.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { existingUser });

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        _usersRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkResultWithTokens()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Test123!"
        };

        var user = new User
        {
            Id = "userId",
            UserName = "testuser",
            Email = loginDto.Email,
            PasswordHash = EventManagementApi.Shared.Helpers.Helpers.Password.Hash(loginDto.Password)
        };

        _usersRepositoryMock.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        _usersRepositoryMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseDict = okResult.Value as dynamic;
        Assert.NotNull(responseDict);

        // Check that token property exists in the response by accessing it as a dictionary/object
        var tokenValue = GetDynamicProperty(responseDict, "token");
        var refreshTokenValue = GetDynamicProperty(responseDict, "refreshToken");

        Assert.NotNull(tokenValue);
        Assert.NotNull(refreshTokenValue);

        _usersRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    // Helper method to get property value from dynamic object
    private static object? GetDynamicProperty(dynamic obj, string propertyName)
    {
        var dictionary = obj as IDictionary<string, object>;
        if (dictionary != null && dictionary.ContainsKey(propertyName))
        {
            return dictionary[propertyName];
        }

        // For ExpandoObject or similar dynamic objects
        try
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
        }
        catch
        {
            // Try direct dynamic access as last resort
            try
            {
                return obj[propertyName];
            }
            catch
            {
                return null;
            }
        }
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Test123!"
        };

        _usersRepositoryMock.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword!"
        };

        var user = new User
        {
            Id = "userId",
            UserName = "testuser",
            Email = loginDto.Email,
            PasswordHash = EventManagementApi.Shared.Helpers.Helpers.Password.Hash("CorrectPassword!")
        };

        _usersRepositoryMock.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task RefreshToken_WithValidTokens_ReturnsNewTokens()
    {
        // Arrange
        var userId = "testUserId";
        var email = "test@example.com";

        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            Email = email,
            RefreshToken = "validRefreshToken",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1).ToString() // Valid expiry
        };

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email)
        };

        var accessToken = EventManagementApi.Shared.Helpers.Helpers.Jwt.GenerateToken(user, _configurationMock.Object);
        var refreshTokenRequest = new RefreshTokenRequestDto
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken
        };

        _usersRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _usersRepositoryMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RefreshToken(refreshTokenRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseDict = okResult.Value as dynamic;
        Assert.NotNull(responseDict);

        // Check that token property exists in the response
        var tokenValue = GetDynamicProperty(responseDict, "token");
        var refreshTokenValue = GetDynamicProperty(responseDict, "refreshToken");

        Assert.NotNull(tokenValue);
        Assert.NotNull(refreshTokenValue);
        Assert.NotEqual(refreshTokenRequest.RefreshToken, refreshTokenValue?.ToString()); // Should get a new refresh token

        _usersRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshTokenRequest = new RefreshTokenRequestDto
        {
            AccessToken = "invalidAccessToken",
            RefreshToken = "someRefreshToken"
        };

        // Act
        var result = await _controller.RefreshToken(refreshTokenRequest);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(401, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_WithExpiredRefreshToken_ReturnsUnauthorized()
    {
        // Arrange
        var userId = "testUserId";
        var email = "test@example.com";

        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            Email = email,
            RefreshToken = "expiredRefreshToken",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1).ToString() // Expired
        };

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email)
        };

        var accessToken = EventManagementApi.Shared.Helpers.Helpers.Jwt.GenerateToken(user, _configurationMock.Object);
        var refreshTokenRequest = new RefreshTokenRequestDto
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken
        };

        _usersRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.RefreshToken(refreshTokenRequest);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(401, statusCodeResult.StatusCode);
    }
}



