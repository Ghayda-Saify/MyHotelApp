namespace MyHotelApp.Shared.DTOs;

public class HotelDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public bool IsFeatured { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public Guid CityId { get; set; }

}