using System.ComponentModel.DataAnnotations;

namespace EventManagementApi.Models.Dto.User;

public class RefreshTokenRequestDto
{
    [Required] public string AccessToken { get; set; } = string.Empty;

    [Required] public string RefreshToken { get; set; } = string.Empty;
}