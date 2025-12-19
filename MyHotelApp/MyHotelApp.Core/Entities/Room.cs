namespace MyHotelApp.Domain.Entities;

public class Room : BaseEntity
{
    public int RoomNumber { get; set; }
    
    // The original price (strikethrough price)
    public decimal BasePrice { get; set; } 
    
    // The actual selling price (discounted)
    public decimal Price { get; set; }

    public string Type { get; set; } = "Standard"; 
    public int AdultsCapacity { get; set; } = 2;
    public int ChildrenCapacity { get; set; } = 0;
    public bool IsAvailable { get; set; } = true;
    public string ImageUrl { get; set; } = string.Empty;

    public Guid HotelId { get; set; }
    public Hotel Hotel { get; set; }
    
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}