namespace MyHotelApp.Domain.Entities;

public class Hotel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
        
    // 1->5 Stars.
    public int StarRating { get; set; } 

    // For "Featured Deals" in Home Page.
    public bool IsFeatured { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

    // Links
    public Guid CityId { get; set; }
    public City? City { get; set; } 

    public List<Room> Rooms { get; set; } = [];
    public List<Booking> Bookings { get; set; } = [];
}