using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHotelApp.Domain.Entities;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using MyHotelApp.Core.Interfaces;

namespace MyHotelApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // All booking actions require login
public class BookingsController(
    IGenericRepository<Booking> bookingRepo,
    IGenericRepository<Room> roomRepo,
    Infrastructure.Data.AppDbContext context,
    IEmailService emailService)
    : ControllerBase
{
    // GET: api/Bookings/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetById(Guid id)
    {
        var booking = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(
                Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(
                    context.Bookings, b => b.Room),
                b => b.Hotel),
            b => b.Id == id);
            
        // Simplified Include syntax if namespace is correct, using explicit for safety in replacement
        // Ideally: context.Bookings.Include(b => b.Hotel).Include(b => b.Room).FirstOrDefaultAsync...
        
        // Let's use cleaner syntax assuming namespaces are imported (they are at top of file needed? I need to check inclusions)
        // File has: using Microsoft.EntityFrameworkCore;
        
        var b = await context.Bookings
            .Include(x => x.Hotel)
            .Include(x => x.Room)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (b == null) return NotFound();

        // Security Check
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (b.UserId.ToString() != userIdString && !User.IsInRole("Admin")) 
            return Unauthorized();

        return Ok(new BookingDto
        {
            Id = b.Id,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            TotalPrice = b.TotalPrice,
            Status = b.Status,
            Remarks = b.Remarks,
            RoomId = b.RoomId,
            HotelId = b.HotelId,
            UserId = b.UserId,
            // Display Props
            HotelName = b.Hotel.Name,
            HotelAddress = b.Hotel.Address,
            RoomType = b.Room.Type,
            GuestName = $"{b.User.FirstName} {b.User.LastName}" 
        });
    }

    // GET: api/Bookings/MyBookings
    [HttpGet("MyBookings")]
    public async Task<ActionResult<List<BookingDto>>> GetMyBookings()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        var userId = Guid.Parse(userIdString);

        var bookings = await bookingRepo.GetAsync(b => b.UserId == userId);
        
        var dtos = bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            TotalPrice = b.TotalPrice,
            Status = b.Status,
            Remarks = b.Remarks,
            RoomId = b.RoomId,
            HotelId = b.HotelId,
            UserId = b.UserId
        }).ToList();

        return Ok(dtos);
    }

    // POST: api/Bookings
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create([FromBody] BookingDto dto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

        // 1. Validate Date
        if (dto.CheckInDate >= dto.CheckOutDate)
            return BadRequest("CheckIn date must be before CheckOut date.");

        if (dto.CheckInDate.Date < DateTime.UtcNow.Date)
            return BadRequest("Cannot book in the past.");

        // 2. Load Room to check Price & Availability
        var room = await roomRepo.GetByIdAsync(dto.RoomId);
        if (room == null) return NotFound("Room not found");

        // 3. Check Availability
        // We need to fetch bookings for this room to check overlap
        // Note: GenericRepo GetAsync might fetch too much if logic isn't push-down. 
        // Assuming simple In-Memory check for now since GenRepo returns List, or optimized if properly implemented.
        var roomBookings = await bookingRepo.GetAsync(b => b.RoomId == dto.RoomId && b.Status != "Cancelled");
        var hasConflict = roomBookings.Any(b => 
            dto.CheckInDate < b.CheckOutDate && 
            dto.CheckOutDate > b.CheckInDate);

        if (hasConflict)
            return BadRequest("Room is not available for these dates.");

        // 4. Calculate Price
        // Assuming Room.Price is per night
        var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
        if (nights < 1) nights = 1; // Fallback? Logic says checkin < checkout so at least 1 day or checkin=checkout? Date check above prevents checkin=checkout.
        
        var totalPrice = room.Price * nights;

        // 5. Create Entity
        var booking = new Booking
        {
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            TotalPrice = totalPrice,
            Status = "Confirmed",
            Remarks = dto.Remarks,
            UserId = Guid.Parse(userIdString),
            RoomId = dto.RoomId,
            HotelId = room.HotelId // Trust Room's HotelId
        };
        
        await bookingRepo.AddAsync(booking);

        // Send confirmation email
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                // Fallback: load user if email claim is missing
                var userObj = await context.Users.FindAsync(booking.UserId);
                userEmail = userObj?.Email;
            }

            if (!string.IsNullOrEmpty(userEmail))
            {
                var body = $"<h1>Booking Confirmed!</h1><p>Thank you for booking with MyHotelApp.</p>" +
                           $"<p><b>Booking ID:</b> {booking.Id}</p>" +
                           $"<p><b>Check-in:</b> {booking.CheckInDate:d}</p>" +
                           $"<p><b>Check-out:</b> {booking.CheckOutDate:d}</p>" +
                           $"<p><b>Total Price:</b> ${totalPrice}</p>";
                await emailService.SendEmailAsync(userEmail, "Booking Confirmation", body);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the booking
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }

        // 6. Return Result
        dto.Id = booking.Id;
        dto.TotalPrice = totalPrice;
        dto.UserId = booking.UserId;
        dto.HotelId = booking.HotelId;
        dto.Status = booking.Status;

        return CreatedAtAction(nameof(GetMyBookings), new { }, dto);
    }

    // GET: api/Bookings/confirmation/{id}/pdf
    [HttpGet("confirmation/{id}/pdf")]
    public IActionResult DownloadConfirmationPdf(Guid id)
    {
        // Mock PDF generation
        var fileContents = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }; // %PDF-
        return File(fileContents, "application/pdf", $"booking_{id}.pdf");
    }

    [HttpPost("confirmation/{id}/email")]
    public async Task<IActionResult> SendConfirmationEmail(Guid id)
    {
        try
        {
            var booking = await context.Bookings
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            var body = $"<h1>Booking Confirmation</h1><p>Here are your booking details:</p>" +
                       $"<p><b>Booking ID:</b> {booking.Id}</p>" +
                       $"<p><b>Total Price:</b> ${booking.TotalPrice}</p>";

            await emailService.SendEmailAsync(booking.User.Email, "Your Booking Confirmation", body);
            
            return Ok(new { message = $"Confirmation email sent for booking {id}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Email sending failed: {ex.Message}");
        }
    }
}
