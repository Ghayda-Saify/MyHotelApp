using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHotelApp.Api.Controllers;
using MyHotelApp.Domain.Entities;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Infrastructure.Data;
using MyHotelApp.Shared.DTOs;
using MyHotelApp.Shared.DTOs.Search;
using Moq;
using Xunit;

namespace MyHotelApp.Tests;

public class HotelsControllerTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IGenericRepository<Hotel>> _mockRepo;
    private readonly HotelsController _controller;

    public HotelsControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockRepo = new Mock<IGenericRepository<Hotel>>();
        _controller = new HotelsController(_mockRepo.Object, _context);
    }

    [Fact]
    public async Task Search_ShouldReturnMatchingHotels()
    {
        // Arrange
        var city = new City { Name = "London", Country = "UK" };
        _context.Cities.Add(city);
        
        var hotel = new Hotel 
        { 
            Name = "Grand London", 
            City = city, 
            Description = "Luxury",
            Rooms = new List<Room> { new Room { Price = 100, AdultsCapacity = 2, ChildrenCapacity = 1 } }
        };
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        var request = new HotelSearchRequestDto { Query = "London", Adults = 2, Children = 1 };

        // Act
        var actionResult = await _controller.Search(request);

        // Assert
        var result = actionResult.Result as OkObjectResult;
        Assert.NotNull(result);
        var response = result.Value as SearchResponseDto;
        Assert.NotNull(response);
        Assert.Single(response.Items);
        Assert.Equal("Grand London", response.Items[0].Name);
    }

    [Fact]
    public async Task Search_PriceFilter_ShouldExcludeExpensiveHotels()
    {
        // Arrange
        var hotel = new Hotel 
        { 
            Name = "Cheap Hotel", 
            Rooms = new List<Room> { new Room { Price = 50, AdultsCapacity = 2 } }
        };
        var expHotel = new Hotel 
        { 
            Name = "Expensive Hotel", 
            Rooms = new List<Room> { new Room { Price = 500, AdultsCapacity = 2 } }
        };
        _context.Hotels.AddRange(hotel, expHotel);
        await _context.SaveChangesAsync();

        var request = new HotelSearchRequestDto { MaxPrice = 100, Adults = 2 };

        // Act
        var actionResult = await _controller.Search(request);

        // Assert
        var response = (actionResult.Result as OkObjectResult)?.Value as SearchResponseDto;
        Assert.NotNull(response);
        Assert.Single(response.Items);
        Assert.Equal("Cheap Hotel", response.Items[0].Name);
    }
}
