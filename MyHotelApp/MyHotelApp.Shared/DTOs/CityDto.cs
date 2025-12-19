namespace MyHotelApp.Shared.DTOs;

public class CityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostOffice { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public int HotelsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
