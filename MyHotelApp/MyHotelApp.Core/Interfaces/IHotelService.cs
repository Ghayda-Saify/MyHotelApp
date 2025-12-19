using MyHotelApp.Shared.DTOs.Search;

namespace MyHotelApp.Domain.Interfaces;

public interface IHotelService
{
    Task<SearchResponseDto> SearchHotelsAsync(HotelSearchRequestDto request);
}