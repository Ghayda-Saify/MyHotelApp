namespace MyHotelApp.Shared.DTOs;
/// <summary>
/// Both API and Client will use this ClassLo
/// </summary>
public class LoginResponse
{
    // The JWT Token that the client must save
    public string Token { get; set; } = string.Empty;

    // How many seconds until it expires 
    public int ExpiresIn { get; set; }
        
    // User's Role, useful for the UI so it hasn't had to ask the Api again
    public string UserRole { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}