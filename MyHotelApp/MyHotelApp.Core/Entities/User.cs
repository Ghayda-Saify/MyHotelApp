namespace MyHotelApp.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
        
    // A Hashed Password
    public string PasswordHash { get; set; } = string.Empty;
        
    // "Admin" or "Customer"
    public string Role { get; set; } = "Customer";

    public List<Booking> Bookings { get; set; } = [];
}