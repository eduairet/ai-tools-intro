using System.ComponentModel.DataAnnotations;

namespace EventManagementApi.Models.Dto.User;

public class RegisterDto
{
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] [MinLength(6)] public string Password { get; set; } = string.Empty;
}