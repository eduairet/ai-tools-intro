using EventManagementApi.Controllers;
using EventManagementApi.Models;
using EventManagementApi.Models.Dto.Event;
using EventManagementApi.Repositories.RepositoryEvents;
using EventManagementApi.Repositories.RepositoryUsers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace AiGeneratedApi.Tests.Controllers;

public class EventsControllerTests : TestBase
{
    private readonly EventsController _controller;
    private readonly Mock<IRepositoryEvents> _eventsRepositoryMock;
    private readonly Mock<IRepositoryUsers> _usersRepositoryMock;

    public EventsControllerTests()
    {
        _eventsRepositoryMock = new Mock<IRepositoryEvents>();
        _usersRepositoryMock = new Mock<IRepositoryUsers>();

        _controller = new EventsController(
            _eventsRepositoryMock.Object,
            _usersRepositoryMock.Object,
            Mapper);
    }

    [Fact]
    public async Task GetAllEvents_ReturnsOkResult_WithListOfEvents()
    {
        // Arrange
        var events = new List<Event>
        {
            new() { Id = "1", Title = "Event 1", Date = "2025-07-15", Location = "Location 1", OwnerId = "user1" },
            new() { Id = "2", Title = "Event 2", Date = "2025-08-15", Location = "Location 2", OwnerId = "user2" }
        };

        _eventsRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(events);

        // Act
        var result = await _controller.GetAllEvents();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedEvents = Assert.IsAssignableFrom<IEnumerable<EventResponseDto>>(okResult.Value);
        Assert.Equal(2, returnedEvents.Count());
    }

    [Fact]
    public async Task GetEvent_WithValidId_ReturnsOkResult_WithEvent()
    {
        // Arrange
        var eventId = "testEventId";
        var eventItem = new Event
        {
            Id = eventId,
            Title = "Test Event",
            Date = "2025-07-15",
            Location = "Test Location",
            OwnerId = "testUser"
        };

        _eventsRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
            .ReturnsAsync(eventItem);

        // Act
        var result = await _controller.GetEvent(eventId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedEvent = Assert.IsType<EventResponseDto>(okResult.Value);
        // Don't compare the IDs directly as our mapping changes them
        Assert.Equal(eventItem.Title, returnedEvent.Title);
    }

    [Fact]
    public async Task GetEvent_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var eventId = "nonexistentId";

        _eventsRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
            .ReturnsAsync((Event?)null);

        // Act
        var result = await _controller.GetEvent(eventId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateEvent_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var userId = "testUserId";
        var userName = "testUser";
        var userEmail = "test@example.com";

        var user = new User { Id = userId, UserName = userName, Email = userEmail };
        var createEventDto = new CreateEventDto
        {
            Title = "New Event",
            Description = "Event Description",
            Date = DateTime.Parse("2025-07-20"),
            Location = "Event Location",
            ImageUrl = "https://example.com/image.jpg"
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateUserPrincipal(userId, userEmail)
            }
        };

        _usersRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _eventsRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Event>()))
            .Returns(Task.CompletedTask);

        _eventsRepositoryMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateEvent(createEventDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedEvent = Assert.IsType<EventResponseDto>(createdAtActionResult.Value);
        Assert.Equal(createEventDto.Title, returnedEvent.Title);
        Assert.Equal(createEventDto.Description, returnedEvent.Description);
        Assert.Equal(createEventDto.Date, returnedEvent.Date);
        Assert.Equal(createEventDto.Location, returnedEvent.Location);
        Assert.Equal(createEventDto.ImageUrl, returnedEvent.ImageUrl);
        Assert.Equal(userId.ToString(), returnedEvent.OwnerId);
    }

    [Fact]
    public async Task UpdateEvent_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var eventId = "testEventId";
        var userId = "testUserId";
        var userEmail = "test@example.com";

        var user = new User { Id = userId, UserName = "testUser", Email = userEmail };
        var existingEvent = new Event
        {
            Id = eventId,
            Title = "Original Title",
            Description = "Original Description",
            Date = "2025-07-20",
            Location = "Original Location",
            OwnerId = userId
        };

        var updateEventDto = new UpdateEventDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Date = DateTime.Parse("2025-08-25"),
            Location = "Updated Location",
            ImageUrl = "https://example.com/updated.jpg"
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateUserPrincipal(userId, userEmail)
            }
        };

        _eventsRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
            .ReturnsAsync(existingEvent);

        _usersRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _eventsRepositoryMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateEvent(eventId, updateEventDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedEvent = Assert.IsType<EventResponseDto>(okResult.Value);
        Assert.Equal(updateEventDto.Title, returnedEvent.Title);
        Assert.Equal(updateEventDto.Description, returnedEvent.Description);
        Assert.Equal(updateEventDto.Date, returnedEvent.Date);
        Assert.Equal(updateEventDto.Location, returnedEvent.Location);
        Assert.Equal(updateEventDto.ImageUrl, returnedEvent.ImageUrl);
    }

    [Fact]
    public async Task UpdateEvent_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var eventId = "nonexistentId";
        var updateEventDto = new UpdateEventDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Date = DateTime.Parse("2025-08-25"),
            Location = "Updated Location"
        };

        _eventsRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
            .ReturnsAsync((Event?)null);

        // Act
        var result = await _controller.UpdateEvent(eventId, updateEventDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateEvent_NotOwnedByUser_ReturnsForbid()
    {
        // Arrange
        var eventId = "testEventId";
        var ownerId = "ownerUserId";
        var currentUserId = "differentUserId";
        var userEmail = "test@example.com";

        var user = new User { Id = currentUserId, UserName = "testUser", Email = userEmail };
        var existingEvent = new Event
        {
            Id = eventId,
            Title = "Original Title",
            Description = "Original Description",
            Date = "2025-07-20",
            Location = "Original Location",
            OwnerId = ownerId // Different from currentUserId
        };

        var updateEventDto = new UpdateEventDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Date = DateTime.Parse("2025-08-25"),
            Location = "Updated Location"
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateUserPrincipal(currentUserId, userEmail)
            }
        };

        _eventsRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
            .ReturnsAsync(existingEvent);

        _usersRepositoryMock.Setup(repo => repo.GetByIdAsync(currentUserId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.UpdateEvent(eventId, updateEventDto);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task DeleteEvent_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var eventId = "testEventId";
        var userId = "testUserId";
        var userEmail = "test@example.com";

        var user = new User { Id = userId, UserName = "testUser", Email = userEmail };
        var existingEvent = new Event
        {
            Id = eventId,
            Title = "Event to Delete",
            Description = "Description",
            Date = "2025-07-20",
            Location = "Location",
            OwnerId = userId
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateUserPrincipal(userId, userEmail)
            }
        };

        _eventsRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
            .ReturnsAsync(existingEvent);

        _usersRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _eventsRepositoryMock.Setup(repo => repo.Remove(existingEvent));

        _eventsRepositoryMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteEvent(eventId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _eventsRepositoryMock.Verify(repo => repo.Remove(existingEvent), Times.Once);
        _eventsRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteEvent_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var eventId = "nonexistentId";

        _eventsRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
            .ReturnsAsync((Event?)null);

        // Act
        var result = await _controller.DeleteEvent(eventId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteEvent_NotOwnedByUser_ReturnsForbid()
    {
        // Arrange
        var eventId = "testEventId";
        var ownerId = "ownerUserId";
        var currentUserId = "differentUserId";
        var userEmail = "test@example.com";

        var user = new User { Id = currentUserId, UserName = "testUser", Email = userEmail };
        var existingEvent = new Event
        {
            Id = eventId,
            Title = "Event to Delete",
            Description = "Description",
            Date = "2025-07-20",
            Location = "Location",
            OwnerId = ownerId // Different from currentUserId
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateUserPrincipal(currentUserId, userEmail)
            }
        };

        _eventsRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
            .ReturnsAsync(existingEvent);

        _usersRepositoryMock.Setup(repo => repo.GetByIdAsync(currentUserId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.DeleteEvent(eventId);

        // Assert
        Assert.IsType<ForbidResult>(result);
        _eventsRepositoryMock.Verify(repo => repo.Remove(It.IsAny<Event>()), Times.Never);
    }
}



