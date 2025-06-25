using Microsoft.AspNetCore.Identity;

namespace EventManagementApi.Models;

public sealed class User : IdentityUser
{
    public string? RefreshToken { get; set; } = string.Empty;
    public string? RefreshTokenExpiryTime { get; set; } = string.Empty;
    public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
    public ICollection<Event> Events { get; set; } = new List<Event>();
}