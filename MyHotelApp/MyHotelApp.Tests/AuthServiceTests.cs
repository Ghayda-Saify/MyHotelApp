using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using MyHotelApp.Domain.Entities;
using MyHotelApp.Infrastructure.Data;
using MyHotelApp.Infrastructure.Services;
using MyHotelApp.Shared.DTOs;
using Xunit;
using Microsoft.EntityFrameworkCore;


namespace MyHotelApp.Tests;

public class AuthServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        
        _mockConfig = new Mock<IConfiguration>();
        // Setup JwtSettings
        var mockJwtSection = new Mock<IConfigurationSection>();
        mockJwtSection.Setup(x => x["SecretKey"]).Returns("SuperSecretKeyForTestingTheHotelApp123!");
        mockJwtSection.Setup(x => x["Issuer"]).Returns("MyHotelApp");
        mockJwtSection.Setup(x => x["Audience"]).Returns("MyHotelAppUser");
        _mockConfig.Setup(x => x.GetSection("JwtSettings")).Returns(mockJwtSection.Object);

        _authService = new AuthService(_context, _mockConfig.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var password = "Password123!";

        // Act
        var result = await _authService.RegisterAsync(user, password);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(string.Empty, result.PasswordHash);
        Assert.Equal(1, await _context.Users.CountAsync());
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("securePass")
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginRequest = new LoginRequest
        {
            Email = "jane@example.com",
            Password = "securePass"
        };

        // Act
        var response = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Token);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var user = new User
        {
            Email = "mark@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctPass")
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginRequest = new LoginRequest
        {
            Email = "mark@example.com",
            Password = "wrongPass"
        };

        // Act
        var response = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.Null(response);
    }
}
