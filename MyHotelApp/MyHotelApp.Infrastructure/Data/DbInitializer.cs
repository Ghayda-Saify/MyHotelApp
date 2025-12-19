using Bogus;
using MyHotelApp.Domain.Entities;
// Add your specific namespaces here

namespace MyHotelApp.Infrastructure.Data;

public class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        // Guard: Stop if users already exist
        if (context.Users.Any()) return;

        // --- STEP 1: Seed Users ---
        
        // A. Create the Admin User with a REAL BCrypt Hash
        var adminUser = new User
        {
            FirstName = "System",
            LastName = "Admin",
            Email = "admin@example.com",
            Role = "Admin",
            // This generates the correct hash for your specific BCrypt version
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!") 
        };
        await context.Users.AddAsync(adminUser);

        // B. Create Random Customers
        // We also need a valid hash here, or the app might crash if you ever try to login as them.
        // We'll calculate it once to keep it fast.
        var defaultCustomerHash = BCrypt.Net.BCrypt.HashPassword("Customer123!");

        var userFaker = new Faker<User>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(u => u.PasswordHash, f => defaultCustomerHash) // Valid hash for everyone
            .RuleFor(u => u.Role, f => "Customer");

        var users = userFaker.Generate(50);
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // --- STEP 2: Seed Cities ---
        var cityFaker = new Faker<City>()
            .RuleFor(c => c.Name, f => f.Address.City())
            .RuleFor(c => c.Country, f => f.Address.Country())
            .RuleFor(c => c.PostOffice, f => f.Address.ZipCode())
            .RuleFor(c => c.ImageUrl, f => f.Image.PicsumUrl());

        var cities = cityFaker.Generate(20);
        await context.Cities.AddRangeAsync(cities);
        await context.SaveChangesAsync();

        // --- STEP 3: Seed Hotels ---
        var hotels = new List<Hotel>();
        var hotelFaker = new Faker<Hotel>()
            .RuleFor(h => h.Name, f => f.Company.CompanyName() + " Hotel")
            .RuleFor(h => h.Description, f => f.Lorem.Paragraph())
            .RuleFor(h => h.Address, f => f.Address.StreetAddress())
            .RuleFor(h => h.Owner, f => f.Name.FullName())
            .RuleFor(h => h.StarRating, f => f.Random.Int(1, 5))
            .RuleFor(h => h.IsFeatured, f => f.Random.Bool(0.2f)) // 20% chance
            .RuleFor(h => h.ImageUrl, f => f.Image.PicsumUrl())
            // Pick a random VALID City ID from the database list we just created
            .RuleFor(h => h.CityId, f => f.PickRandom(cities).Id);

        hotels = hotelFaker.Generate(50);
        await context.Hotels.AddRangeAsync(hotels);
        await context.SaveChangesAsync();

        // --- STEP 4: Seed Rooms ---
        // I want multiple rooms per hotel, so I loop through hotels
        var rooms = new List<Room>();
        var roomFaker = new Faker<Room>()
            .RuleFor(r => r.RoomNumber, f => f.Random.Int(100, 999))
            .RuleFor(r => r.Price, f => decimal.Parse(f.Commerce.Price(50, 500)))
            .RuleFor(r => r.Type, f => f.PickRandom("Luxury", "Budget", "Boutique", "Standard"))
            .RuleFor(r => r.AdultsCapacity, f => f.Random.Int(1, 4))
            .RuleFor(r => r.ChildrenCapacity, f => f.Random.Int(0, 2))
            .RuleFor(r => r.IsAvailable, f => true) // Default to true initially
            .RuleFor(r => r.ImageUrl, f => f.Image.PicsumUrl());

        foreach (var hotel in hotels)
        {
            // Generate 10 rooms for THIS hotel
            var hotelRooms = roomFaker.Clone()
                .RuleFor(r => r.HotelId, hotel.Id)
                .Generate(10);
            
            rooms.AddRange(hotelRooms);
        }
        await context.Rooms.AddRangeAsync(rooms);
        await context.SaveChangesAsync();

        // --- STEP 5: Seed Bookings ---
        // I create bookings using valid Users, Hotels, and Rooms
        var bookingFaker = new Faker<Booking>()
            .RuleFor(b => b.CheckInDate, f => f.Date.Future()) // Future date
            .RuleFor(b => b.CheckOutDate, (f, b) => b.CheckInDate.AddDays(f.Random.Int(1, 7)))
            .RuleFor(b => b.TotalPrice, f => decimal.Parse(f.Commerce.Price(200, 1000)))
            .RuleFor(b => b.Status, f => "Confirmed")
            .RuleFor(b => b.Remarks, f => f.Lorem.Sentence())
            .RuleFor(b => b.UserId, f => f.PickRandom(users).Id);

        var bookings = new List<Booking>();
        
        // Generate 100 bookings
        for (int i = 0; i < 100; i++)
        {
            // We must pick a random Room and ensure we use its HotelId
            var randomRoom = rooms[new Random().Next(rooms.Count)];
            
            var booking = bookingFaker.Clone()
                .RuleFor(b => b.RoomId, randomRoom.Id)
                .RuleFor(b => b.HotelId, randomRoom.HotelId) // Must match the room's hotel
                .Generate();
                
            bookings.Add(booking);
        }
        
        
        await context.Bookings.AddRangeAsync(bookings);
        await context.SaveChangesAsync();
    }
}