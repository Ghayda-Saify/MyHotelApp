namespace MyHotelApp.Shared.DTOs.Home;

public class FeaturedDealDto
{
    public Guid HotelId { get; set; }
    public string HotelName { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string CityName { get; set; }
    public int StarRating { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public int DiscountPercentage { get; set; }
}