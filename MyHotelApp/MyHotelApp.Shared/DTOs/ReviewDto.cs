namespace MyHotelApp.Shared.DTOs;

public class ReviewDto
{
    public Guid Id { get; set; }
    public string AppUserName { get; set; } = "Anonymous";
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
