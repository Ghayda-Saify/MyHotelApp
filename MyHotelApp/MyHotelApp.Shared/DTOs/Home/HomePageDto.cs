namespace MyHotelApp.Shared.DTOs.Home;

public class HomePageDto
{
    public List<FeaturedDealDto> FeaturedDeals { get; set; } = new();
    public List<TrendingDestinationDto> TrendingDestinations { get; set; } = new();
}