using Microsoft.AspNetCore.Identity;

namespace EventManagementApi.Models.User
{
    public class User : IdentityUser
    {
        // Additional fields can be added here if needed
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
} 