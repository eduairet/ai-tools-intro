using System.Net.Http.Json;
using EventManagementApi.Models.Dto.Event;
using EventManagementApi.Models.Dto.User;
using Microsoft.Extensions.DependencyInjection;
using EventManagementApi.Data;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;

namespace AiGeneratedApi.Tests.Integration;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IServiceProvider _serviceProvider;

    public ApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _serviceProvider = factory.Services;

        // Initialize test data if needed
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        SeedTestData(dbContext).Wait();
    }

    private async Task SeedTestData(AppDbContext dbContext)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        // Add any test data here if needed for specific tests
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task RegisterUser_ThenLogin_ThenCreateEvent_EndToEndTest()
    {
        // Register a new user
        var registerDto = new RegisterDto
        {
            Email = $"integration_{Guid.NewGuid()}@example.com",
            Password = "Test123!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/users/register", registerDto);
        registerResponse.EnsureSuccessStatusCode();

        // Login with the new user
        var loginDto = new LoginDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/users/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(loginResult);
        Assert.NotNull(loginResult.Token);

        // Set the token for authorized requests
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

        // Create a new event
        var createEventDto = new CreateEventDto
        {
            Title = "Integration Test Event",
            Description = "Created in an integration test",
            Date = DateTime.Parse("2025-07-30"),
            Location = "Test Location"
        };

        var createEventResponse = await _client.PostAsJsonAsync("/api/v1/events", createEventDto);
        createEventResponse.EnsureSuccessStatusCode();

        var eventResult = await createEventResponse.Content.ReadFromJsonAsync<EventResponseDto>();
        Assert.NotNull(eventResult);
        Assert.Equal(createEventDto.Title, eventResult.Title);

        // Get the created event
        var getEventResponse = await _client.GetAsync($"/api/v1/events/{eventResult.Id}");
        getEventResponse.EnsureSuccessStatusCode();

        var retrievedEvent = await getEventResponse.Content.ReadFromJsonAsync<EventResponseDto>();
        Assert.NotNull(retrievedEvent);
        Assert.Equal(createEventDto.Title, retrievedEvent.Title);
    }

    [Fact]
    public async Task GetEvents_WithoutAuthentication_ReturnsOk()
    {
        // Anyone should be able to get events without authentication
        var response = await _client.GetAsync("/api/v1/events");
        response.EnsureSuccessStatusCode();

        var events = await response.Content.ReadFromJsonAsync<List<EventResponseDto>>();
        Assert.NotNull(events);
    }

    private class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
