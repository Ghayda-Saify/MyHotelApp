using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace MyHotelApp.Client.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient http)
    {
        _localStorage = localStorage;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
    }

    public void NotifyUserAuthentication(string token)
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        _http.DefaultRequestHeaders.Authorization = null;
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            foreach (var kvp in keyValuePairs)
            {
                var key = kvp.Key;
                var value = kvp.Value;

                // Handle Roles
                if (IsRoleClaim(key))
                {
                    AddClaims(claims, ClaimTypes.Role, value);
                }
                // Handle Names
                else if (IsNameClaim(key))
                {
                    AddClaims(claims, ClaimTypes.Name, value);
                }
                // Handle Emails
                else if (IsEmailClaim(key))
                {
                    AddClaims(claims, ClaimTypes.Email, value);
                }
                // Handle Other
                else
                {
                    AddClaims(claims, key, value);
                }
            }
        }

        return claims;
    }

    private bool IsRoleClaim(string key)
    {
        return key == "role" || 
               key == "roles" || 
               key == ClaimTypes.Role || 
               key == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    }

    private bool IsNameClaim(string key)
    {
        return key == "unique_name" || 
               key == "name" || 
               key == "sub" ||
               key == ClaimTypes.Name || 
               key == ClaimTypes.NameIdentifier ||
               key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" ||
               key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    }

    private bool IsEmailClaim(string key)
    {
        return key == "email" || 
               key == "emailaddress" || 
               key == ClaimTypes.Email || 
               key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    }

    private void AddClaims(List<Claim> claims, string type, object value)
    {
        if (value is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    claims.Add(new Claim(type, item.ToString()));
                }
            }
            else
            {
                claims.Add(new Claim(type, element.ToString()));
            }
        }
        else
        {
            claims.Add(new Claim(type, value?.ToString() ?? string.Empty));
        }
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
