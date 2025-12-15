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
    public async Task<ActionResult<List<HotelDTO>>> GetAll()
    {
        var hotels = await hotelRepo.GetAllAsync();

        // Convert Entity -> DTO manually (Simple & Fast)
        var dtos = hotels.Select(h => new HotelDTO
        {
            Id = h.Id,
            Name = h.Name,
            Address = h.Address,
            StarRating = h.StarRating,
            IsFeatured = h.IsFeatured
        }).ToList();

        return Ok(dtos);
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