namespace MyHotelApp.Shared.DTOs.Search;

public class SearchResponseDto
{
    public List<HotelSearchResultDto> Items { get; set; } = new();
    public int TotalCount { get; set; } // Needed for "Page 1 of 5"
}