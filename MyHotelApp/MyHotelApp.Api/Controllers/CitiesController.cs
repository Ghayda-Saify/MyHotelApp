using Microsoft.AspNetCore.Mvc;
using MyHotelApp.Domain.Entities;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MyHotelApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CitiesController(IGenericRepository<City> cityRepo, MyHotelApp.Infrastructure.Data.AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CityDto>>> GetAll()
    {
        var cities = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(
            context.Cities, c => c.Hotels).ToListAsync();
        var dtos = cities.Select(c => new CityDto
        {
            Id = c.Id,
            Name = c.Name,
            Country = c.Country,
            PostOffice = c.PostOffice,
            ImageUrl = c.ImageUrl,
            HotelsCount = c.Hotels.Count,
            CreatedAt = c.CreatedAt,
            ModifiedAt = c.ModifiedAt
        }).ToList();
        return Ok(dtos);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CityDto>> GetById(Guid id)
    {
        var city = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(context.Cities, c => c.Hotels),
            c => c.Id == id);

        if (city == null) return NotFound();

        return Ok(new CityDto
        {
            Id = city.Id,
            Name = city.Name,
            Country = city.Country,
            PostOffice = city.PostOffice,
            ImageUrl = city.ImageUrl,
            HotelsCount = city.Hotels.Count,
            CreatedAt = city.CreatedAt,
            ModifiedAt = city.ModifiedAt
        });
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<ActionResult<CityDto>> Create([FromBody] CityDto dto)
    {
        var city = new City
        {
            Name = dto.Name,
            Country = dto.Country,
            PostOffice = dto.PostOffice,
            ImageUrl = dto.ImageUrl
        };

        await cityRepo.AddAsync(city);

        dto.Id = city.Id;
        return CreatedAtAction(nameof(GetById), new { id = city.Id }, dto);
    }

    [HttpPut("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CityDto dto)
    {
        if (id != dto.Id) return BadRequest();

        var city = await cityRepo.GetByIdAsync(id);
        if (city == null) return NotFound();

        city.Name = dto.Name;
        city.Country = dto.Country;
        city.PostOffice = dto.PostOffice;
        city.ImageUrl = dto.ImageUrl;
        city.ModifiedAt = DateTime.UtcNow;

        await cityRepo.UpdateAsync(city);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var city = await cityRepo.GetByIdAsync(id);
        if (city == null) return NotFound();

        await cityRepo.DeleteAsync(city);
        return NoContent();
    }
}
