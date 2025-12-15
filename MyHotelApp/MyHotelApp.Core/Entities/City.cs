namespace MyHotelApp.Domain.Entities;

public class City : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostOffice { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty; 

    // Navigation: One City has many Hotels
    public List<Hotel> Hotels { get; set; } = new();   
}