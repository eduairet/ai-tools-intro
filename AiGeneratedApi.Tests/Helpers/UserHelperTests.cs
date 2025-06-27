using System.Security.Claims;
using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryUsers;
using EventManagementApi.Shared.Helpers;
using Moq;
using Xunit;

namespace AiGeneratedApi.Tests.Helpers;

public class UserHelperTests : TestBase
{
    private readonly Mock<IRepositoryUsers> _usersRepositoryMock;

    public UserHelperTests()
    {
        _usersRepositoryMock = new Mock<IRepositoryUsers>();
    }

    [Fact]
    public async Task GetAndValidateCurrentUserAsync_WithValidUser_ReturnsSuccess()
    {
        // Arrange
        var userId = "testUserId";
        var userEmail = "test@example.com";

        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            Email = userEmail
        };

        var claimsPrincipal = CreateUserPrincipal(userId, userEmail);

        _usersRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var (success, returnedUser, errorResult) = await UserHelper.GetAndValidateCurrentUserAsync(
            claimsPrincipal, _usersRepositoryMock.Object);

        // Assert
        Assert.True(success);
        Assert.NotNull(returnedUser);
        Assert.Equal(userId, returnedUser.Id);
        Assert.Null(errorResult);
    }

    [Fact]
    public async Task GetAndValidateCurrentUserAsync_WithNonexistentUser_ReturnsFailure()
    {
        // Arrange
        var userId = "nonexistentUserId";
        var userEmail = "nonexistent@example.com";

        var claimsPrincipal = CreateUserPrincipal(userId, userEmail);

        _usersRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var (success, returnedUser, errorResult) = await UserHelper.GetAndValidateCurrentUserAsync(
            claimsPrincipal, _usersRepositoryMock.Object);

        // Assert
        Assert.False(success);
        Assert.Null(returnedUser);
        Assert.NotNull(errorResult);
    }

    [Fact]
    public async Task GetAndValidateCurrentUserAsync_WithNullPrincipal_ReturnsFailure()
    {
        // Create a mock ClaimsPrincipal with no claims
        var emptyPrincipal = new ClaimsPrincipal();

        // Act
        var (success, returnedUser, errorResult) = await UserHelper.GetAndValidateCurrentUserAsync(
            emptyPrincipal, _usersRepositoryMock.Object);

        // Assert
        Assert.False(success);
        Assert.Null(returnedUser);
        Assert.NotNull(errorResult);
    }
}


