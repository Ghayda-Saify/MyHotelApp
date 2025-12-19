namespace MyHotelApp.Shared.DTOs;

public class HotelDetailsDto : HotelDto
{
    public List<RoomDto> Rooms { get; set; } = new();
    public List<ReviewDto> Reviews { get; set; } = new();
    public List<string> Gallery { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
}
