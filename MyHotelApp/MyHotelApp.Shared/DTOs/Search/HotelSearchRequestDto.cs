namespace MyHotelApp.Shared.DTOs.Search;

// What the user sends TO the server
public class HotelSearchRequestDto
{
    public string? Query { get; set; } // e.g., "Paris" or "Hilton"
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int Adults { get; set; } = 2;
    
    // Filters we will build in the UI later
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? StarRating { get; set; }
    
    // Pagination (Load 10 at a time)
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

