using Microsoft.EntityFrameworkCore;
using MyHotelApp.Domain.Entities;

namespace MyHotelApp.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // This tells EF Core: "Whenever you see a decimal (like Price), 
        // store it as Money (18 digits total, 2 after the dot)."
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }
        // If a Hotel is deleted, do NOT auto-delete Bookings.
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Hotel)
            .WithMany(h => h.Bookings)
            .HasForeignKey(b => b.HotelId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // If a Room is deleted, do NOT auto-delete Bookings.
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany()
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Room>()
            .HasMany(r => r.Bookings)    // A Room has many Bookings
            .WithOne(b => b.Room)        // A Booking has one Room
            .HasForeignKey(b => b.RoomId) // Use the EXISTING "RoomId" column
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    
}