using Microsoft.AspNetCore.Mvc;
using MyHotelApp.Domain.Entities;
using MyHotelApp.Domain.Interfaces;
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
            IsFeatured = h.IsFeatured
        }).ToList();

        return Ok(dtos);
    }
}