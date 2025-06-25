using System;
using System.Collections.Generic;

namespace EventManagementApi.Models;

public class EventRegistration
{
    public string Id { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public virtual Event Event { get; set; } = new();
    public virtual User User { get; set; } = new();
}
