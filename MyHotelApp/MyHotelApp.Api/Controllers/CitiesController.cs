using Microsoft.AspNetCore.Mvc;
using MyHotelApp.Domain.Entities;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Shared.DTOs;

namespace MyHotelApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CitiesController(IGenericRepository<City> cityRepo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CityDto>>> GetAll()
    {
        var cities = await cityRepo.GetAllAsync();
        var dtos = cities.Select(c => new CityDto
        {
            Id = c.Id,
            Name = c.Name,
            Country = c.Country
        }).ToList();
        return Ok(dtos);
    }
}
