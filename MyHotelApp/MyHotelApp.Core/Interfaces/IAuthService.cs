using MyHotelApp.Shared.DTOs;

namespace MyHotelApp.Domain.Interfaces;

public interface IAuthService
{
    // Returns the LoginResponse (Token) or null if login fails
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    
    //TODO
    // Helper to register a user (Optional for now, but good to have)
    //Task<User?> RegisterAsync(User user, string password);
}