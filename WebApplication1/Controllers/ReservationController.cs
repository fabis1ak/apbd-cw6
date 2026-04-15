using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetReservations([FromQuery] DateTime? date, [FromQuery] string? status, [FromQuery] int? roomId)
    {
        var query = DataStore.Reservations.AsQueryable();

        if (date.HasValue) query = query.Where(r => r.Date.Date == date.Value.Date);
        if (!string.IsNullOrEmpty(status)) query = query.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        if (roomId.HasValue) query = query.Where(r => r.RoomId == roomId.Value);

        return Ok(query.ToList());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetReservation(int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null) return NotFound($"Rezerwacja o ID {id} nie została znaleziona.");

        return Ok(reservation);
    }

    [HttpPost]
    public IActionResult CreateReservation([FromBody] Reservation reservation)
    {
        if (reservation.EndTime <= reservation.StartTime)
            return BadRequest("Czas zakończenia musi być późniejszy niż czas rozpoczęcia.");

        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
        if (room == null) return NotFound($"Sala o ID {reservation.RoomId} nie istnieje.");
        if (!room.IsActive) return BadRequest("Wybrana sala jest obecnie nieaktywna.");

        if (IsOverlapping(reservation))
            return Conflict("Rezerwacja koliduje czasowo z inną rezerwacją w tej sali.");

        reservation.Id = DataStore.Reservations.Any() ? DataStore.Reservations.Max(r => r.Id) + 1 : 1;
        DataStore.Reservations.Add(reservation);

        return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateReservation(int id, [FromBody] Reservation updatedReservation)
    {
        if (updatedReservation.EndTime <= updatedReservation.StartTime)
            return BadRequest("Czas zakończenia musi być późniejszy niż czas rozpoczęcia.");

        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null) return NotFound($"Rezerwacja o ID {id} nie została znaleziona.");

        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == updatedReservation.RoomId);
        if (room == null) return NotFound($"Sala o ID {updatedReservation.RoomId} nie istnieje.");
        if (!room.IsActive) return BadRequest("Wybrana sala jest obecnie nieaktywna.");

        if (IsOverlapping(updatedReservation, id))
            return Conflict("Aktualizacja koliduje czasowo z inną istniejącą rezerwacją w tej sali.");

        reservation.RoomId = updatedReservation.RoomId;
        reservation.OrganizerName = updatedReservation.OrganizerName;
        reservation.Topic = updatedReservation.Topic;
        reservation.Date = updatedReservation.Date;
        reservation.StartTime = updatedReservation.StartTime;
        reservation.EndTime = updatedReservation.EndTime;
        reservation.Status = updatedReservation.Status;

        return Ok(reservation);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteReservation(int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null) return NotFound($"Rezerwacja o ID {id} nie została znaleziona.");

        DataStore.Reservations.Remove(reservation);
        return NoContent();
    }

    private bool IsOverlapping(Reservation newReservation, int? excludeId = null)
    {
        return DataStore.Reservations.Any(r =>
            r.Id != excludeId &&
            r.RoomId == newReservation.RoomId &&
            r.Date.Date == newReservation.Date.Date &&
            r.StartTime < newReservation.EndTime && 
            newReservation.StartTime < r.EndTime);  
    }
}