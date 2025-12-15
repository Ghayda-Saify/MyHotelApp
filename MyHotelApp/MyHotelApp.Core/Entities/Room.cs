namespace MyHotelApp.Domain.Entities;

public class Room : BaseEntity
{
    public int RoomNumber { get; set; }
    public decimal Price { get; set; }
        
    // "Luxury", "Budget", "Boutique"
    public string Type { get; set; } = "Standard"; 
        
    // Capacity
    public int AdultsCapacity { get; set; } = 2;
    public int ChildrenCapacity { get; set; } = 0;

    public bool IsAvailable { get; set; } = true;
    public string ImageUrl { get; set; } = string.Empty;

    // Links
    public Guid HotelId { get; set; }
    public Hotel Hotel { get; set; }
}