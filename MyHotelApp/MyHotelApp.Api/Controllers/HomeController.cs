using Microsoft.AspNetCore.Mvc;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Shared.DTOs.Home;

namespace MyHotelApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController(IHomeService homeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HomePageDto>> GetHomeData()
    {
        var data = await homeService.GetHomePageDataAsync();
        return Ok(data);
    }
}