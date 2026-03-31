
using System.ComponentModel.DataAnnotations;

namespace MyHotelApp.Domain.Entities;

public abstract class BaseEntity
{
    [Key] // The primary key
    public Guid Id { get; set; } = Guid.NewGuid(); // Auto-generates a unique ID

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Auto-sets the date
    public DateTime? ModifiedAt { get; set; }
}
