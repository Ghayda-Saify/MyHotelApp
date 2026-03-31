namespace MyHotelApp.Domain.Entities;

public class Review : BaseEntity
{
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; } // e.g., 1 to 5

    // Links
    public Guid HotelId { get; set; }
    public Hotel Hotel { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}