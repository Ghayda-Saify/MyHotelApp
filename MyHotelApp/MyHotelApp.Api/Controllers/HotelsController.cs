using Microsoft.AspNetCore.Mvc;
using MyHotelApp.Domain.Entities;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Domain.Specifications;
using MyHotelApp.Shared.DTOs;

namespace MyHotelApp.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class HotelsController(IGenericRepository<Hotel> hotelRepo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<HotelDto>>> GetAll()
    {
        var hotels = await hotelRepo.GetAllAsync();

        // Convert Entity -> DTO manually (Simple & Fast)
        var dtos = hotels.Select(h => new HotelDto
        {
            Id = h.Id,
            Name = h.Name,
            Address = h.Address,
            StarRating = h.StarRating,
            IsFeatured = h.IsFeatured,
            Description = h.Description,
            ImageUrl = h.ImageUrl,
            CityId = h.CityId
        }).ToList();

        return Ok(dtos);
    }

    [HttpPost]
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
            CityId = dto.CityId
        };
        
        await hotelRepo.AddAsync(hotel);
        
        return CreatedAtAction(nameof(GetAll), new { id = hotel.Id }, dto);
    }
    
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Hotel>>> Search([FromQuery] string query)
    {
        // 1. Get the filter logic
        var spec = HotelSearchSpec.FilterByCityOrName(query);

        // 2. Ask DB for matches
        var hotels = await hotelRepo.GetAsync(spec);

        // 3. Return
        return Ok(hotels);
    }
    
}