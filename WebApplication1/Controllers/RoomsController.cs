using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRooms([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector, [FromQuery] bool? activeOnly)
    {
        var query = DataStore.Rooms.AsQueryable();

        if (minCapacity.HasValue) query = query.Where(r => r.Capacity >= minCapacity.Value);
        if (hasProjector.HasValue) query = query.Where(r => r.HasProjector == hasProjector.Value);
        if (activeOnly == true) query = query.Where(r => r.IsActive);

        return Ok(query.ToList());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetRoom(int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room == null) return NotFound($"Sala o ID {id} nie została znaleziona.");

        return Ok(room);
    }

    [HttpGet("building/{buildingCode}")]
    public IActionResult GetRoomsByBuilding(string buildingCode)
    {
        var rooms = DataStore.Rooms
            .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(rooms);
    }

    [HttpPost]
    public IActionResult CreateRoom([FromBody] Room room)
    {
        room.Id = DataStore.Rooms.Any() ? DataStore.Rooms.Max(r => r.Id) + 1 : 1;
        DataStore.Rooms.Add(room);

        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateRoom(int id, [FromBody] Room updatedRoom)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room == null) return NotFound($"Sala o ID {id} nie została znaleziona.");

        room.Name = updatedRoom.Name;
        room.BuildingCode = updatedRoom.BuildingCode;
        room.Floor = updatedRoom.Floor;
        room.Capacity = updatedRoom.Capacity;
        room.HasProjector = updatedRoom.HasProjector;
        room.IsActive = updatedRoom.IsActive;

        return Ok(room);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteRoom(int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room == null) return NotFound($"Sala o ID {id} nie została znaleziona.");

        if (DataStore.Reservations.Any(res => res.RoomId == id && res.Date >= DateTime.Today))
            return Conflict("Nie można usunąć sali, ponieważ istnieją dla niej przyszłe rezerwacje.");

        DataStore.Rooms.Remove(room);
        return NoContent();
    }
}