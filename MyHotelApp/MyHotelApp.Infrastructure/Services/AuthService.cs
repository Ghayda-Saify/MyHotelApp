using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyHotelApp.Domain.Entities;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Infrastructure.Data;
using MyHotelApp.Shared.DTOs;

namespace MyHotelApp.Infrastructure.Services;
/// <summary>
/// The Security Engine, It's responsible for verifying who a user is
/// and issuing them the JWT so they can access the rest of the app.
/// </summary>
/// <param name="context"> To Connect with the DB and insure the identity of the User.</param>
/// <param name="configuration"> To read secret values from appsettings.json.</param>
public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            // 1. Find User
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return null;

            // 2. Verify Password (using BCrypt)
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid) return null;

            // 3. Generate JWT Token
            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Token = token,
                UserRole = user.Role,
                ExpiresIn = 3600, // == 1 hr
                IsSuccess = true, 
                Message = "Login successful"
            };
        }

        public async Task<User?> RegisterAsync(User user, string password)
        {
            // Hash the password before saving!
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                    ),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
}