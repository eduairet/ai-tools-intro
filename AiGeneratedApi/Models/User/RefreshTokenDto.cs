using System.ComponentModel.DataAnnotations;

namespace EventManagementApi.Models.User
{
    public class RefreshTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
} 