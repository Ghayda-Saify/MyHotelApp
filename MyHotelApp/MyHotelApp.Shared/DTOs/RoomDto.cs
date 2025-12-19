namespace MyHotelApp.Shared.DTOs;

public class RoomDto
{
    public Guid Id { get; set; }
    public int RoomNumber { get; set; }
    public decimal BasePrice { get; set; } 
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty;
    public int AdultsCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public bool IsAvailable { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Guid HotelId { get; set; }
    
    // Display
    public string? HotelName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
