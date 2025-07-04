namespace EventManagementApi.Models.Dto.Event;

public class EventResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string? OwnerName { get; set; } = string.Empty;
}