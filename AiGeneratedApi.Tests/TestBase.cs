using System.Security.Claims;
using AutoMapper;
using EventManagementApi.Data;
using EventManagementApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AiGeneratedApi.Tests;

public abstract class TestBase
{
    protected readonly AppDbContext DbContext;
    protected readonly IMapper Mapper;
    protected readonly Mock<UserManager<User>> UserManagerMock;
    protected readonly Mock<SignInManager<User>> SignInManagerMock;

    protected TestBase()
    {
        // Set up in-memory database with a static name to ensure test isolation but shared state when needed
        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .EnableSensitiveDataLogging()
            .Options;

        DbContext = new AppDbContext(dbOptions);

        // Clear database to ensure clean state
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();

        // Set up AutoMapper with our mock mapper
        Mapper = MockMapper.CreateMockMapper();

        // Set up mocks for UserManager and SignInManager
        var userStoreMock = new Mock<IUserStore<User>>();
        UserManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();

        SignInManagerMock = new Mock<SignInManager<User>>(
            UserManagerMock.Object,
            contextAccessorMock.Object,
            userPrincipalFactoryMock.Object,
            null!, null!, null!, null!);
    }

    protected static ClaimsPrincipal CreateUserPrincipal(string userId, string email)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email)
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    protected async Task<User> CreateTestUser(string id = "testUserId", string email = "test@example.com", string username = "testuser")
    {
        var user = new User
        {
            Id = id,
            Email = email,
            UserName = username,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = username.ToUpperInvariant(),
            EmailConfirmed = true
        };

        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        return user;
    }

    protected async Task<Event> CreateTestEvent(string id = "testEventId", string title = "Test Event",
        string ownerId = "testUserId", string date = "2025-07-01", string location = "Test Location")
    {
        var eventItem = new Event
        {
            Id = id,
            Title = title,
            Description = "Test Description",
            Date = date,
            Location = location,
            OwnerId = ownerId,
            ImageUrl = "https://example.com/image.jpg"
        };

        await DbContext.Events.AddAsync(eventItem);
        await DbContext.SaveChangesAsync();

        return eventItem;
    }

    protected async Task<EventRegistration> CreateTestRegistration(string id = "testRegistrationId",
        string eventId = "testEventId", string userId = "testUserId")
    {
        var registration = new EventRegistration
        {
            Id = id,
            EventId = eventId,
            UserId = userId
        };

        await DbContext.EventRegistrations.AddAsync(registration);
        await DbContext.SaveChangesAsync();

        return registration;
    }
}


