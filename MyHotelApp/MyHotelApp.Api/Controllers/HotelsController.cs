using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHotelApp.Domain.Entities;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Infrastructure.Data; // Ensure this points to your DbContext
using MyHotelApp.Shared.DTOs;
using MyHotelApp.Shared.DTOs.Search;

namespace MyHotelApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController(IGenericRepository<Hotel> hotelRepo, AppDbContext context) : ControllerBase
{
    // 1. GET ALL (Simple List)
    [HttpGet]
    public async Task<ActionResult<List<HotelDto>>> GetAll()
    {
        var hotels = await context.Hotels
            .Include(h => h.City)
            .Include(h => h.Rooms)
            .ToListAsync();

        // Convert Entity -> DTO
        var dtos = hotels.Select(h => new HotelDto
        {
            HotelId = h.Id,
            Name = h.Name,
            Address = h.Address,
            StarRating = h.StarRating,
            IsFeatured = h.IsFeatured,
            Description = h.Description,
            ImageUrl = h.ImageUrl,
            CityId = h.CityId,
            CityName = h.City?.Name ?? "Unknown",
            Owner = h.Owner,
            RoomsCount = h.Rooms.Count,
            CreatedAt = h.CreatedAt,
            ModifiedAt = h.ModifiedAt
        }).ToList();

        return Ok(dtos);
    }

    // 1.1 GET DETAILS
    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDetailsDto>> GetById(Guid id)
    {
        var hotel = await context.Hotels
            .Include(h => h.City)
            .Include(h => h.Rooms)
            .Include(h => h.Reviews)
                .ThenInclude(r => r.User)
            .Include(h => h.Gallery)
            .Include(h => h.Amenities)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null) return NotFound();

        // Convert to Detail DTO
        var dto = new HotelDetailsDto
        {
            HotelId = hotel.Id,
            Name = hotel.Name,
            Address = hotel.Address,
            StarRating = hotel.StarRating,
            IsFeatured = hotel.IsFeatured,
            Description = hotel.Description,
            ImageUrl = hotel.ImageUrl,
            CityId = hotel.CityId,
            CityName = hotel.City?.Name ?? "Unknown",
            PricePerNight = hotel.Rooms.Any() ? hotel.Rooms.Min(r => r.Price) : 0,

            // Lists
            Rooms = hotel.Rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                BasePrice = r.BasePrice,
                Price = r.Price,
                Type = r.Type,
                AdultsCapacity = r.AdultsCapacity,
                ChildrenCapacity = r.ChildrenCapacity,
                IsAvailable = r.IsAvailable,
                ImageUrl = r.ImageUrl,
                HotelId = r.HotelId
            }).ToList(),

            Reviews = hotel.Reviews.Select(rev => new ReviewDto
            {
                Id = rev.Id,
                Rating = rev.Rating,
                Comment = rev.Comment,
                CreatedAt = rev.CreatedAt,
                AppUserName = (rev.User != null) ? $"{rev.User.FirstName} {rev.User.LastName}" : "Anonymous"
            }).ToList(),

            Gallery = hotel.Gallery.Select(g => g.Url).ToList(),
            Amenities = hotel.Amenities.Select(a => a.Name).ToList()
        };

        return Ok(dto);
    }

    // 2. CREATE
    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<ActionResult<HotelDto>> Create([FromBody] HotelDto dto)
    {
        var hotel = new Hotel
        {
            Name = dto.Name,
            Address = dto.Address,
            StarRating = dto.StarRating,
            IsFeatured = dto.IsFeatured,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            CityId = dto.CityId,
            Owner = dto.Owner
        };
        
        await hotelRepo.AddAsync(hotel);
        
        dto.HotelId = hotel.Id;
        return CreatedAtAction(nameof(GetAll), new { id = hotel.Id }, dto);
    }

    // 2.1 UPDATE
    [HttpPut("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] HotelDto dto)
    {
        if (id != dto.HotelId) return BadRequest();

        var hotel = await hotelRepo.GetByIdAsync(id);
        if (hotel == null) return NotFound();

        hotel.Name = dto.Name;
        hotel.Address = dto.Address;
        hotel.StarRating = dto.StarRating;
        hotel.IsFeatured = dto.IsFeatured;
        hotel.Description = dto.Description;
        hotel.ImageUrl = dto.ImageUrl;
        hotel.CityId = dto.CityId;
        hotel.Owner = dto.Owner;
        hotel.ModifiedAt = DateTime.UtcNow;

        await hotelRepo.UpdateAsync(hotel);
        return NoContent();
    }

    // 2.2 DELETE
    [HttpDelete("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var hotel = await hotelRepo.GetByIdAsync(id);
        if (hotel == null) return NotFound();

        await hotelRepo.DeleteAsync(hotel);
        return NoContent();
    }
    
    // 3. SEARCH (The Fix)
    [HttpGet("search")]
    public async Task<ActionResult<SearchResponseDto>> Search([FromQuery] HotelSearchRequestDto request)
    {
        // Start Query using DbContext directly for complex filtering
        var query = context.Hotels
            .Include(h => h.City)
            .Include(h => h.Rooms)
            .ThenInclude(r => r.Bookings)
            .AsQueryable();

        // A. Text Search (Name or City)
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var term = request.Query.ToLower();
            query = query.Where(h => h.Name.ToLower().Contains(term) || 
                                     (h.City != null && h.City.Name.ToLower().Contains(term)));
        }

        // B. Capacity Filter (Must have room for X adults)
        if (request.Adults > 0 || request.Children > 0)
        {
            query = query.Where(h => h.Rooms.Any(r => 
                r.AdultsCapacity >= request.Adults && 
                r.ChildrenCapacity >= request.Children)); // <--- NEW CHECK
        }

        // C. Star Rating
        if (request.StarRating.HasValue)
        {
            query = query.Where(h => h.StarRating >= request.StarRating.Value);
        }

        // D. Max Price
        if (request.MaxPrice.HasValue)
        {
             query = query.Where(h => h.Rooms.Any(r => r.Price <= request.MaxPrice.Value));
        }

        // E. Property Type (New Requirement)
        if (request.PropertyTypes != null && request.PropertyTypes.Any())
        {
            query = query.Where(h => request.PropertyTypes.Contains(h.Description)); // Mocking Type mapping via Description for now if no Type property exists
        }

        // F. Amenities (New Requirement)
        if (request.Amenities != null && request.Amenities.Any())
        {
            foreach (var amenity in request.Amenities)
            {
                query = query.Where(h => h.Amenities.Any(a => a.Name == amenity));
            }
        }

        // E. Date Availability (The complex part)
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

        // F. Sorting
        query = request.SortOrder switch
        {
            "StarsDesc" => query.OrderByDescending(h => h.StarRating),
            _ => query.OrderBy(h => h.Rooms.Min(r => r.Price)) // Default: Cheapest first
        };

        // G. Pagination
        var totalCount = await query.CountAsync();
        var hotels = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        // H. Convert to DTOs
        var resultList = hotels.Select(h => 
        {
            // Calculate price for display
            var cheapestRoom = h.Rooms
                .Where(r => r.AdultsCapacity >= request.Adults)
                .OrderBy(r => r.Price)
                .FirstOrDefault();

            return new HotelDto
            {
                HotelId = h.Id,
                Name = h.Name,
                Description = h.Description,
                ImageUrl = h.ImageUrl,
                StarRating = h.StarRating,
                IsFeatured = h.IsFeatured,
                Address = h.Address,
                
                // New Fields needed for UI
                CityName = h.City?.Name ?? "Unknown",
                PricePerNight = cheapestRoom?.Price ?? 0
            };
        }).ToList();

        return Ok(new SearchResponseDto
        {
            Items = resultList,
            TotalCount = totalCount
        });
    }
}