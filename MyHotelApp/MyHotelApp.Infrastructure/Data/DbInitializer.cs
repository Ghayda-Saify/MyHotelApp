using MyHotelApp.Domain.Entities;

namespace MyHotelApp.Infrastructure.Data;

public class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. Ensure the database is created
        await context.Database.EnsureCreatedAsync();

        // 2. Check if we already have hotels. If yes, stop here.
        if (context.Hotels.Any()) return; 

        // 3. Create Dummy Data
        var city = new City 
        { 
            Name = "Paris", 
            Country = "France", 
            PostOffice = "75001",
            ImageUrl = "https://dummyimage.com/600x400/000/fff&text=Paris"
        };

        var hotels = new List<Hotel>
        {
            new Hotel
            {
                Name = "Grand Luxury Hotel",
                Address = "123 Champs-Elysees",
                Description = "The best view in Paris",
                StarRating = 5,
                IsFeatured = true,
                City = city,
                ImageUrl = "https://dummyimage.com/600x400/000/fff&text=Luxury"
            },
            new Hotel
            {
                Name = "Cozy Budget Inn",
                Address = "45 Backstreet Alley",
                Description = "Good for backpackers",
                StarRating = 2,
                IsFeatured = false,
                City = city,
                ImageUrl = "https://dummyimage.com/600x400/000/fff&text=Budget"
            }
        };

        // 4. Save to DB
        await context.Hotels.AddRangeAsync(hotels);
        await context.SaveChangesAsync();
    }
}