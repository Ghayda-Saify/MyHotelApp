using System.ComponentModel.DataAnnotations;

namespace MyHotelApp.Shared.DTOs;
/// <summary>
/// Both API and Client will use this Class.
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;
}