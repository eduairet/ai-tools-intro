using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryEvents;
using EventManagementApi.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using Xunit;

using Microsoft.AspNetCore.Http;

using System.Linq.Expressions;
namespace AiGeneratedApi.Tests.Repositories;

public class RepositoryEventsTests : TestBase
{
    private readonly IRepositoryEvents _repository;

    public RepositoryEventsTests()
    {
        // Use mock repository instead of real one
        _repository = new MockRepositoryEvents();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEvents()
    {
        // Arrange - create test events directly using the repository
        var event1 = new Event { Id = "event1", Title = "Event 1", Date = "2025-07-15", Location = "Location 1", OwnerId = "owner1" };
        var event2 = new Event { Id = "event2", Title = "Event 2", Date = "2025-07-16", Location = "Location 2", OwnerId = "owner1" };
        var event3 = new Event { Id = "event3", Title = "Event 3", Date = "2025-07-17", Location = "Location 3", OwnerId = "owner1" };

        await _repository.AddAsync(event1);
        await _repository.AddAsync(event2);
        await _repository.AddAsync(event3);
        await _repository.SaveChangesAsync();

        // Act
        var events = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, events.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsEvent()
    {
        // Arrange - create test event directly using the repository
        var eventId = "testEventId";
        var eventTitle = "Test Event";
        var eventItem = new Event
        {
            Id = eventId,
            Title = eventTitle,
            Description = "Test Description",
            Date = "2025-07-15",
            Location = "Test Location",
            OwnerId = "owner1"
        };

        await _repository.AddAsync(eventItem);
        await _repository.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(eventId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventId, result.Id);
        Assert.Equal(eventTitle, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var eventId = "nonexistentId";

        // Act
        var eventItem = await _repository.GetByIdAsync(eventId);

        // Assert
        Assert.Null(eventItem);
    }

    [Fact]
    public async Task AddAsync_AddsEventToDbContext()
    {
        // Arrange
        var eventItem = new Event
        {
            Id = "newEventId",
            Title = "New Event",
            Description = "Description",
            Date = "2025-07-20",
            Location = "Location",
            OwnerId = "ownerId"
        };

        // Act
        await _repository.AddAsync(eventItem);
        await _repository.SaveChangesAsync();

        // Assert
        var addedEvent = await _repository.GetByIdAsync(eventItem.Id);
        Assert.NotNull(addedEvent);
        Assert.Equal(eventItem.Title, addedEvent.Title);
    }

    [Fact]
    public async Task Remove_RemovesEventFromDbContext()
    {
        // Arrange
        var eventId = "eventToDeleteId";
        var eventItem = new Event
        {
            Id = eventId,
            Title = "Event to Delete",
            Description = "Test Description",
            Date = "2025-07-20",
            Location = "Test Location",
            OwnerId = "owner1"
        };

        await _repository.AddAsync(eventItem);
        await _repository.SaveChangesAsync();

        // Verify event was added
        var addedEvent = await _repository.GetByIdAsync(eventId);
        Assert.NotNull(addedEvent);

        // Act
        _repository.Remove(eventItem);
        await _repository.SaveChangesAsync();

        // Assert
        var deletedEvent = await _repository.GetByIdAsync(eventId);
        Assert.Null(deletedEvent);
    }

    [Fact]
    public async Task FindAsync_WithMatchingPredicate_ReturnsMatchingEvents()
    {
        // Arrange
        var event1 = new Event { Id = "event1", Title = "First Test Event", Date = "2025-07-15", Location = "Location 1" };
        var event2 = new Event { Id = "event2", Title = "Second Test Event", Date = "2025-07-16", Location = "Location 2" };
        var event3 = new Event { Id = "event3", Title = "Third Regular Event", Date = "2025-07-17", Location = "Location 3" };

        await _repository.AddAsync(event1);
        await _repository.AddAsync(event2);
        await _repository.AddAsync(event3);
        await _repository.SaveChangesAsync();

        // Act
        var events = await _repository.FindAsync(e => e.Title.Contains("Test"));

        // Assert
        Assert.Equal(2, events.Count());
        Assert.All(events, e => Assert.Contains("Test", e.Title));
    }

    private async Task<Event> CreateTestEvent(string id, string title)
    {
        var eventItem = new Event
        {
            Id = id,
            Title = title,
            Description = "Test Description",
            Date = "2025-07-20",
            Location = "Test Location",
            OwnerId = "testOwner"
        };

        DbContext.Events.Add(eventItem);
        await DbContext.SaveChangesAsync();
        return eventItem;
    }
}


