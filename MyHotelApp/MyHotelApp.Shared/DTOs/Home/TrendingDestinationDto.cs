namespace MyHotelApp.Shared.DTOs.Home;

public class TrendingDestinationDto
{
    public Guid CityId { get; set; }
    public string CityName { get; set; }
    public string CountryName { get; set; }
    public string ImageUrl { get; set; }
}