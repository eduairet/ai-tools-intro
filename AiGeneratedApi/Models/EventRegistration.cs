namespace EventManagementApi.Models;

public sealed class EventRegistration
{
    public string Id { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public Event Event { get; set; } = new();
    public User User { get; set; } = new();
}
