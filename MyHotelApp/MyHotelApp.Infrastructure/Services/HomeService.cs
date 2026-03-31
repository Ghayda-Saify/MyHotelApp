using Microsoft.EntityFrameworkCore;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Infrastructure.Data;
using MyHotelApp.Shared.DTOs.Home;

namespace MyHotelApp.Infrastructure.Services;

public class HomeService(AppDbContext context) : IHomeService
{
    public async Task<HomePageDto> GetHomePageDataAsync()
    {
        var response = new HomePageDto();

        // --- 1. Fetch Featured Deals ---
        // Requirement: Top 3-5 hotels marked as "Featured" [cite: 49]
        var featuredHotels = await context.Hotels
            .Include(h => h.City)
            .Include(h => h.Rooms)
            .Where(h => h.IsFeatured)
            .Take(5)
            .ToListAsync();

        response.FeaturedDeals = featuredHotels.Select(h => {
            // Logic: Find the cheapest room to show "Price starting from..."
            var cheapestRoom = h.Rooms.OrderBy(r => r.Price).FirstOrDefault();
            
            // Handle nulls if a hotel has no rooms yet
            var originalPrice = cheapestRoom?.BasePrice ?? 0;
            var currentPrice = cheapestRoom?.Price ?? 0;
            
            return new FeaturedDealDto
            {
                HotelId = h.Id,
                HotelName = h.Name,
                // Truncate description for the card view
                Description = h.Description?.Length > 100 ? h.Description.Substring(0, 100) + "..." : h.Description,
                ImageUrl = h.ImageUrl,
                CityName = h.City?.Name ?? "Unknown",
                StarRating = h.StarRating,
                OriginalPrice = originalPrice,
                DiscountedPrice = currentPrice,
                // Calculate discount % safely
                DiscountPercentage = originalPrice > 0 
                    ? (int)((1 - (currentPrice / originalPrice)) * 100) 
                    : 0
            };
        }).ToList();

        // --- 2. Fetch Trending Destinations ---
        // Requirements: Top 5 cities based on booking count [cite: 57]
        var trendingData = await context.Bookings
            .Include(b => b.Hotel)
            .ThenInclude(h => h.City)
            .Where(b => b.Hotel != null && b.Hotel.City != null)
            .GroupBy(b => b.Hotel.CityId)
            .Select(g => new 
            { 
                City = g.First().Hotel.City, 
                BookingCount = g.Count() 
            })
            .OrderByDescending(x => x.BookingCount)
            .Take(5)
            .ToListAsync();

        response.TrendingDestinations = trendingData.Select(item => new TrendingDestinationDto
        {
            CityId = item.City.Id,
            CityName = item.City.Name,
            CountryName = item.City.Country,
            ImageUrl = item.City.ImageUrl
        }).ToList();

        return response;
    }
}