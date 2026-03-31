namespace MyHotelApp.Shared.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public Guid RoomId { get; set; }
    public Guid HotelId { get; set; }
    public Guid UserId { get; set; }

    // Display Fields (Optional / Populated on GetById)
    public string? HotelName { get; set; }
    public string? HotelAddress { get; set; }
    public string? RoomType { get; set; }
    public string? GuestName { get; set; }
}
