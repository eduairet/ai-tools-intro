namespace EventManagementApi.Models;

public sealed class Event
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
    public User? Owner { get; set; }
}
