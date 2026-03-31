using System.Net.Http.Json;
using Blazored.LocalStorage;
using MyHotelApp.Shared.DTOs;

namespace MyHotelApp.Client.Services;


/// <summary>
/// 
/// </summary>
/// <param name="httpClient"></param>
/// <param name="localStorage"></param>
public class AuthService(HttpClient httpClient, ILocalStorageService localStorage) : IAuthService
{
    public async Task<LoginResponse> Login(LoginRequest loginRequest)
    {
        // 1. Call the API.
        var result = await httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

        // 2. Check if it failed.
        if (!result.IsSuccessStatusCode)
        {
            var errorContent = await result.Content.ReadFromJsonAsync<LoginResponse>();
            return errorContent ?? new LoginResponse 
            { 
                IsSuccess = false, 
                Message = "Invalid email or password" 
            };
        }

        // 3. Read the Token.
        var response = await result.Content.ReadFromJsonAsync<LoginResponse>();

        // 4. Save to LocalStorage.
        await localStorage.SetItemAsync("authToken", response!.Token);

        return response;
    }

    public async Task Logout()
    {
        await localStorage.RemoveItemAsync("authToken");
    }
}