namespace MyHotelApp.Domain.Entities;

public class HotelImage : BaseEntity
{
    public string Url { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty; // e.g., "Lobby View"

    public Guid HotelId { get; set; }
    public Hotel Hotel { get; set; }
}