namespace MyHotelApp.Shared.DTOs;

public class HotelDto
{
    public Guid HotelId { get; set; } 

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public bool IsFeatured { get; set; }
    public string? PropertyType { get; set; }

    // --- Added Back for "Create Hotel" Form ---
    public string Address { get; set; } = string.Empty;
    public Guid CityId { get; set; } // Required for the Dropdown

    // --- Read-Only / Display Properties ---
    public string CityName { get; set; } = string.Empty; 
    public decimal PricePerNight { get; set; }

    public string Owner { get; set; } = string.Empty;
    public int RoomsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
