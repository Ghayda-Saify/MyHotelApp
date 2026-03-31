using MyHotelApp.Shared.DTOs.Home;

namespace MyHotelApp.Domain.Interfaces;

public interface IHomeService
{
    Task<HomePageDto> GetHomePageDataAsync();
}