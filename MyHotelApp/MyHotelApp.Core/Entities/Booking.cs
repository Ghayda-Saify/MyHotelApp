namespace MyHotelApp.Domain.Entities;

public class Booking : BaseEntity
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
        
    // "Confirmed", "Cancelled", "Completed".
    public string Status { get; set; } = "Confirmed";
        
    // Special Requests. 
    public string? Remarks { get; set; } 

    // Links (Who booked what?)
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid RoomId { get; set; }
    public Room Room { get; set; }
    
    public Guid HotelId { get; set; } 
    public Hotel Hotel { get; set; }
}