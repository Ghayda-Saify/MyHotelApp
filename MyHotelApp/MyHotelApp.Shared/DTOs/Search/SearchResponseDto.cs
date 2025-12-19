namespace MyHotelApp.Shared.DTOs.Search;

public class SearchResponseDto
{
    public List<HotelDto> Items { get; set; } = [];
    public int TotalCount { get; set; } // Needed for "Page 1 of 5"
}