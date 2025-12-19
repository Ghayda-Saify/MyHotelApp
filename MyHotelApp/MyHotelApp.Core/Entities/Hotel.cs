namespace MyHotelApp.Domain.Entities;

public class Hotel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public int StarRating { get; set; } 
    public bool IsFeatured { get; set; }
    
    //  Map Coordinates for the "Interactive Map"
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    // Links
    public Guid CityId { get; set; }
    public City? City { get; set; } 

    public List<Room> Rooms { get; set; } = [];
    public List<Booking> Bookings { get; set; } = [];

    // Navigation Properties
    public List<Review> Reviews { get; set; } = [];     // For "Guest Reviews"
    public List<HotelImage> Gallery { get; set; } = []; // For "Visual Gallery"
    public List<Amenity> Amenities { get; set; } = [];  // For "Search Filters"
}