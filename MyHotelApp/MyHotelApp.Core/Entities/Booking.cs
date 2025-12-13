namespace MyHotelApp.Domain.Entities;

public class Booking : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } // Navigation Property

    public Guid RoomId { get; set; }
    public Room Room { get; set; } // Navigation Property

    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
        
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Confirmed"; // "Confirmed", "Cancelled"
}