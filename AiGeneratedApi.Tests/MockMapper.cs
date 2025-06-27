using AiGeneratedApi.Tests.Mocks;
using AutoMapper;
using EventManagementApi.Models;
using EventManagementApi.Models.Dto.Event;
using EventManagementApi.Models.Dto.EventRegistration;
using EventManagementApi.Models.Dto.User;
using Moq;

namespace AiGeneratedApi.Tests;

public static class MockMapper
{
    public static IMapper CreateMockMapper()
    {
        // Create a mock mapper that intercepts the mapping calls and returns our mock DTOs
        var mockMapper = new Mock<IMapper>();

        // Setup mocks for Event -> EventResponseDto
        mockMapper.Setup(m => m.Map<EventResponseDto>(It.IsAny<Event>()))
            .Returns((Event src) =>
            {
                // Create a mock DTO with matching properties
                var mockDto = new MockEventResponseDto
                {
                    Id = src.Id,
                    Title = src.Title,
                    Description = src.Description ?? string.Empty,
                    Date = src.Date,
                    Location = src.Location,
                    ImageUrl = src.ImageUrl,
                    OwnerId = src.OwnerId,
                    OwnerName = src.Owner?.UserName
                };

                // Convert it to the expected EventResponseDto with proper type conversions
                return new EventResponseDto
                {
                    Id = string.IsNullOrEmpty(mockDto.Id) ? Guid.Empty : (Guid.TryParse(mockDto.Id, out var guidId) ? guidId : Guid.Empty),
                    Title = mockDto.Title,
                    Description = mockDto.Description,
                    Date = string.IsNullOrEmpty(mockDto.Date) ? DateTime.MinValue : (DateTime.TryParse(mockDto.Date, out var date) ? date : DateTime.MinValue),
                    Location = mockDto.Location,
                    ImageUrl = mockDto.ImageUrl,
                    OwnerId = mockDto.OwnerId,
                    OwnerName = mockDto.OwnerName
                };
            });

        // Setup mocks for IEnumerable<Event> -> IEnumerable<EventResponseDto>
        mockMapper.Setup(m => m.Map<IEnumerable<EventResponseDto>>(It.IsAny<IEnumerable<Event>>()))
            .Returns((IEnumerable<Event> src) =>
            {
                var result = new List<EventResponseDto>();
                foreach (var eventItem in src)
                {
                    result.Add(new EventResponseDto
                    {
                        Id = string.IsNullOrEmpty(eventItem.Id) ? Guid.Empty : (Guid.TryParse(eventItem.Id, out var guidId) ? guidId : Guid.Empty),
                        Title = eventItem.Title,
                        Description = eventItem.Description ?? string.Empty,
                        Date = string.IsNullOrEmpty(eventItem.Date) ? DateTime.MinValue : (DateTime.TryParse(eventItem.Date, out var date) ? date : DateTime.MinValue),
                        Location = eventItem.Location,
                        ImageUrl = eventItem.ImageUrl,
                        OwnerId = eventItem.OwnerId,
                        OwnerName = eventItem.Owner?.UserName
                    });
                }
                return result;
            });

        // Setup mocks for EventRegistration -> EventRegistrationResponseDto
        mockMapper.Setup(m => m.Map<EventRegistrationResponseDto>(It.IsAny<EventRegistration>()))
            .Returns((EventRegistration src) =>
            {
                var mockDto = new MockEventRegistrationResponseDto
                {
                    Id = src.Id,
                    EventId = src.EventId,
                    EventTitle = src.Event?.Title ?? string.Empty,
                    UserId = src.UserId,
                    UserName = src.User?.UserName ?? string.Empty
                };

                return new EventRegistrationResponseDto
                {
                    Id = string.IsNullOrEmpty(mockDto.Id) ? Guid.Empty : (Guid.TryParse(mockDto.Id, out var guidId) ? guidId : Guid.Empty),
                    EventId = string.IsNullOrEmpty(mockDto.EventId) ? Guid.Empty : (Guid.TryParse(mockDto.EventId, out var guidEventId) ? guidEventId : Guid.Empty),
                    EventTitle = mockDto.EventTitle,
                    UserId = mockDto.UserId,
                    UserName = mockDto.UserName
                };
            });

        // Setup mocks for IEnumerable<EventRegistration> -> IEnumerable<EventRegistrationResponseDto>
        mockMapper.Setup(m => m.Map<IEnumerable<EventRegistrationResponseDto>>(It.IsAny<IEnumerable<EventRegistration>>()))
            .Returns((IEnumerable<EventRegistration> src) =>
            {
                var result = new List<EventRegistrationResponseDto>();
                foreach (var registration in src)
                {
                    result.Add(new EventRegistrationResponseDto
                    {
                        Id = string.IsNullOrEmpty(registration.Id) ? Guid.Empty : (Guid.TryParse(registration.Id, out var guidId) ? guidId : Guid.Empty),
                        EventId = string.IsNullOrEmpty(registration.EventId) ? Guid.Empty : (Guid.TryParse(registration.EventId, out var guidEventId) ? guidEventId : Guid.Empty),
                        EventTitle = registration.Event?.Title ?? string.Empty,
                        UserId = registration.UserId,
                        UserName = registration.User?.UserName ?? string.Empty
                    });
                }
                return result;
            });

        // Setup other necessary mappings (CreateEventDto -> Event, etc.)
        mockMapper.Setup(m => m.Map<Event>(It.IsAny<CreateEventDto>()))
            .Returns((CreateEventDto src) => new Event
            {
                Title = src.Title,
                Description = src.Description,
                Date = src.Date.ToString("yyyy-MM-dd"),
                Location = src.Location,
                ImageUrl = src.ImageUrl
            });

        mockMapper.Setup(m => m.Map<Event>(It.IsAny<UpdateEventDto>()))
            .Returns((UpdateEventDto src) => new Event
            {
                Title = src.Title,
                Description = src.Description,
                Date = src.Date.ToString("yyyy-MM-dd"),
                Location = src.Location,
                ImageUrl = src.ImageUrl
            });

        // Setup UpdateEventDto mapping (for updating existing Event)
        mockMapper.Setup(m => m.Map(It.IsAny<UpdateEventDto>(), It.IsAny<Event>()))
            .Callback((UpdateEventDto src, Event dest) =>
            {
                dest.Title = src.Title;
                dest.Description = src.Description;
                dest.Date = src.Date.ToString("yyyy-MM-dd");
                dest.Location = src.Location;
                if (!string.IsNullOrEmpty(src.ImageUrl))
                {
                    dest.ImageUrl = src.ImageUrl;
                }
            });

        // Setup RegisterDto -> User mapping
        mockMapper.Setup(m => m.Map<User>(It.IsAny<RegisterDto>()))
            .Returns((RegisterDto src) => new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = src.Email,
                UserName = src.Email,
                NormalizedEmail = src.Email.ToUpperInvariant(),
                NormalizedUserName = src.Email.ToUpperInvariant()
            });

        // Setup User -> UserResponseDto mapping
        mockMapper.Setup(m => m.Map<UserResponseDto>(It.IsAny<User>()))
            .Returns((User src) => new UserResponseDto
            {
                Id = src.Id ?? string.Empty,
                Email = src.Email ?? string.Empty,
                UserName = src.UserName ?? string.Empty
            });

        return mockMapper.Object;
    }
}
