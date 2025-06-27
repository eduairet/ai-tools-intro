namespace AiGeneratedApi.Tests.Mocks;

// Mock DTOs that match the model's types for testing
public class MockEventResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string? OwnerName { get; set; } = string.Empty;
}

public class MockEventRegistrationResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string EventTitle { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
