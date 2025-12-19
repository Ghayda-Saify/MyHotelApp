namespace MyHotelApp.Shared.DTOs.Search;
// What the server sends BACK to the user
public class HotelSearchResultDto
{
    public Guid HotelId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string CityName { get; set; }
    public int StarRating { get; set; }
    public decimal PricePerNight { get; set; } // Cheapest available room
    public bool IsFeatured { get; set; }
}