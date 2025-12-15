using MyHotelApp.Shared.DTOs;

namespace MyHotelApp.Client.Services;


/// <summary>
/// 
/// </summary>
public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest loginRequest);
    Task Logout();
}