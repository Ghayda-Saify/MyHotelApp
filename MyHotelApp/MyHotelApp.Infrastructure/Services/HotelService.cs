using Microsoft.EntityFrameworkCore;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Infrastructure.Data;
using MyHotelApp.Shared.DTOs; 
using MyHotelApp.Shared.DTOs.Search;

namespace MyHotelApp.Infrastructure.Services;

public class HotelService(AppDbContext context) : IHotelService
{
    public async Task<SearchResponseDto> SearchHotelsAsync(HotelSearchRequestDto request)
{
    Console.WriteLine($"--- SEARCH STARTED: Query='{request.Query}', Adults={request.Adults}, Date={request.CheckInDate} ---");

    // 1. Start with ALL hotels (Include necessary data)
    var query = context.Hotels
        .Include(h => h.City)
        .Include(h => h.Rooms)
        .ThenInclude(r => r.Bookings)
        .AsQueryable();

    var initialCount = await query.CountAsync();
    Console.WriteLine($"1. Total Hotels in DB: {initialCount}");

    // 2. Text Search (Handle NULL City safely)
    if (!string.IsNullOrWhiteSpace(request.Query))
    {
        var term = request.Query.ToLower().Trim();
        query = query.Where(h => 
            h.Name.ToLower().Contains(term) || 
            (h.City != null && h.City.Name.ToLower().Contains(term)) // <--- Fixed potential crash here
        );
    }
    
    var countAfterText = await query.CountAsync();
    Console.WriteLine($"2. After Text Search ('{request.Query}'): {countAfterText}");

    // 3. Filter by Capacity (Adults)
    // CRITICAL: Many DBs have default AdultsCapacity = 0. This filter often kills results.
    if (request.Adults > 0)
    {
        query = query.Where(h => h.Rooms.Any(r => r.AdultsCapacity >= request.Adults));
    }
    
    var countAfterCapacity = await query.CountAsync();
    Console.WriteLine($"3. After Capacity (Needs {request.Adults} adults): {countAfterCapacity}");

    // 4. Availability Check
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
    
    var countAfterDate = await query.CountAsync();
    Console.WriteLine($"4. After Availability Check: {countAfterDate}");

    // 5. Pagination & Final Fetch
    var totalCount = await query.CountAsync();
    
    var hotels = await query
        .OrderByDescending(h => h.IsFeatured) // Ensure consistent ordering
        .Skip((request.Page - 1) * request.PageSize)
        .Take(request.PageSize)
        .ToListAsync();

    Console.WriteLine($"5. Final Result Count: {hotels.Count}");

    // 6. Mapping
    var resultList = hotels.Select(h => 
    {
        var cheapestRoom = h.Rooms
            .Where(r => r.AdultsCapacity >= request.Adults)
            .OrderBy(r => r.Price)
            .FirstOrDefault();

        // If no room fits the capacity, fallback to ANY room just to show a price (safer)
        if (cheapestRoom == null) cheapestRoom = h.Rooms.OrderBy(r => r.Price).FirstOrDefault();

        return new HotelDto
        {
            HotelId = h.Id,
            Name = h.Name,
            Description = h.Description,
            ImageUrl = h.ImageUrl,
            StarRating = h.StarRating,
            IsFeatured = h.IsFeatured,
            CityName = h.City?.Name ?? "No City",
            PricePerNight = cheapestRoom?.Price ?? 0
        };
    }).ToList();

    return new SearchResponseDto
    {
        Items = resultList,
        TotalCount = totalCount
    };
}
}