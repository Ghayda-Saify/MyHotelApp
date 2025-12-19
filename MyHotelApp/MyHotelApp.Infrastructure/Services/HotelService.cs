using Microsoft.EntityFrameworkCore;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Infrastructure.Data;
using MyHotelApp.Shared.DTOs.Search;

namespace MyHotelApp.Infrastructure.Services;

public class HotelService(AppDbContext context) : IHotelService
{
    public async Task<SearchResponseDto> SearchHotelsAsync(HotelSearchRequestDto request)
    {
        // Start with ALL hotels
        var query = context.Hotels
            .Include(h => h.City)
            .Include(h => h.Rooms)
            .ThenInclude(r => r.Bookings) // Load bookings to check availability
            .AsQueryable();

        // 1. Text Search (City or Hotel Name)
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var term = request.Query.ToLower();
            query = query.Where(h => h.Name.ToLower().Contains(term) || 
                                     h.City.Name.ToLower().Contains(term));
        }

        // 2. Filter by Capacity (Must hold enough people)
        query = query.Where(h => h.Rooms.Any(r => r.AdultsCapacity >= request.Adults));

        // 3. Filter by Star Rating
        if (request.StarRating.HasValue)
        {
            query = query.Where(h => h.StarRating >= request.StarRating.Value);
        }

        // 4. Availability Check (The complex part)
        // If user picked dates, we exclude hotels where ALL suitable rooms are booked.
        if (request.CheckInDate.HasValue && request.CheckOutDate.HasValue)
        {
            var checkIn = request.CheckInDate.Value;
            var checkOut = request.CheckOutDate.Value;

            query = query.Where(h => h.Rooms.Any(r => 
                r.AdultsCapacity >= request.Adults && 
                !r.Bookings.Any(b => 
                    checkIn < b.CheckOutDate && 
                    checkOut > b.CheckInDate
                )
            ));
        }

        // 5. Get Total Count (for pagination)
        var totalCount = await query.CountAsync();

        // 6. Get Page Data
        var hotels = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        // 7. Convert to DTOs
        var resultList = hotels.Select(h => 
        {
            // Calculate price based on the cheapest room that fits the criteria
            var room = h.Rooms
                .Where(r => r.AdultsCapacity >= request.Adults)
                .OrderBy(r => r.Price)
                .FirstOrDefault();

            return new HotelSearchResultDto
            {
                HotelId = h.Id,
                Name = h.Name,
                Description = h.Description,
                ImageUrl = h.ImageUrl,
                CityName = h.City?.Name ?? "Unknown",
                StarRating = h.StarRating,
                IsFeatured = h.IsFeatured,
                PricePerNight = room?.Price ?? 0
            };
        }).ToList();

        return new SearchResponseDto
        {
            Items = resultList,
            TotalCount = totalCount
        };
    }
}