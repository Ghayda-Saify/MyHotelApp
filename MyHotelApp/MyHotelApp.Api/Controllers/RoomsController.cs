using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHotelApp.Domain.Entities;
using MyHotelApp.Domain.Interfaces;
using MyHotelApp.Shared.DTOs;

namespace MyHotelApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomsController(
    IGenericRepository<Room> roomRepo,
    Infrastructure.Data.AppDbContext context) 
    : ControllerBase
{
    // GET: api/Rooms
    [HttpGet]
    public async Task<ActionResult<List<RoomDto>>> GetAll()
    {
        // Use Context to Include Hotel
        var rooms = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(context.Rooms, r => r.Hotel).ToListAsync();
        
        var dtos = rooms.Select(r => new RoomDto
        {
            Id = r.Id,
            RoomNumber = r.RoomNumber,
            BasePrice = r.BasePrice,
            Price = r.Price,
            Type = r.Type,
            AdultsCapacity = r.AdultsCapacity,
            ChildrenCapacity = r.ChildrenCapacity,
            IsAvailable = r.IsAvailable,
            ImageUrl = r.ImageUrl,
            HotelId = r.HotelId,
            HotelName = r.Hotel?.Name ?? "Unknown",
            CreatedAt = r.CreatedAt,
            ModifiedAt = r.ModifiedAt
        }).ToList();

        return Ok(dtos);
    }

    // GET: api/Rooms/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetById(Guid id)
    {
        var r = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(context.Rooms, x => x.Hotel),
                x => x.Id == id);
                
        if (r == null) return NotFound();

        var dto = new RoomDto
        {
            Id = r.Id,
            RoomNumber = r.RoomNumber,
            BasePrice = r.BasePrice,
            Price = r.Price,
            Type = r.Type,
            AdultsCapacity = r.AdultsCapacity,
            ChildrenCapacity = r.ChildrenCapacity,
            IsAvailable = r.IsAvailable,
            ImageUrl = r.ImageUrl,
            HotelId = r.HotelId,
            HotelName = r.Hotel?.Name ?? "Unknown"
        };
        return Ok(dto);
    }

    // POST: api/Rooms
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomDto>> Create([FromBody] RoomDto dto)
    {
        var room = new Room
        {
            RoomNumber = dto.RoomNumber,
            BasePrice = dto.BasePrice,
            Price = dto.Price,
            Type = dto.Type,
            AdultsCapacity = dto.AdultsCapacity,
            ChildrenCapacity = dto.ChildrenCapacity,
            IsAvailable = dto.IsAvailable,
            ImageUrl = dto.ImageUrl,
            HotelId = dto.HotelId
        };
        
        // Use Repo for Write is fine, or context. Using Repo to keep it consistent with original injection if preferred,
        // but now we have context injected so either works. Let's stick to repo for Writes as implemented before unless I remove it.
        // I kept repo in constructor.
        
        await roomRepo.AddAsync(room);

        dto.Id = room.Id;
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, dto);
    }

    // PUT: api/Rooms/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RoomDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");

        var room = await roomRepo.GetByIdAsync(id);
        if (room == null) return NotFound();

        room.RoomNumber = dto.RoomNumber;
        room.BasePrice = dto.BasePrice;
        room.Price = dto.Price;
        room.Type = dto.Type;
        room.AdultsCapacity = dto.AdultsCapacity;
        room.ChildrenCapacity = dto.ChildrenCapacity;
        room.IsAvailable = dto.IsAvailable;
        room.ImageUrl = dto.ImageUrl;
        room.HotelId = dto.HotelId;
        room.ModifiedAt = DateTime.UtcNow;

        await roomRepo.UpdateAsync(room);

        return NoContent();
    }

    // DELETE: api/Rooms/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var room = await roomRepo.GetByIdAsync(id);
        if (room == null) return NotFound();

        await roomRepo.DeleteAsync(room);

        return NoContent();
    }
}
