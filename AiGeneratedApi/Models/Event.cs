namespace EventManagementApi.Models;

public class Event
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Date { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string OwnerId { get; set; } = string.Empty;

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

    public virtual User Owner { get; set; } = new();
}
