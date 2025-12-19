namespace MyHotelApp.Domain.Entities;

public class Amenity : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., "Swimming Pool", "Free WiFi"
    public string Description { get; set; } = string.Empty;

    // Navigation: Many-to-Many relationship (A hotel has many amenities, an amenity belongs to many hotels)
    public List<Hotel> Hotels { get; set; } = new();
}