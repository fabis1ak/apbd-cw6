namespace WebApplication1.Data;

using System;
using System.Collections.Generic;
using WebApplication1.Models;

public static class DataStore
{
    public static List<Room> Rooms { get; set; } = new();
    public static List<Reservation> Reservations { get; set; } = new();

    static DataStore()
    {
       
        Rooms.Add(new Room { Id = 1, Name = "Lab 101", BuildingCode = "A", Floor = 1, Capacity = 15, HasProjector = true, IsActive = true });
        Rooms.Add(new Room { Id = 2, Name = "Aula Główna", BuildingCode = "A", Floor = 0, Capacity = 100, HasProjector = true, IsActive = true });
        Rooms.Add(new Room { Id = 3, Name = "Salka Spotkań 1", BuildingCode = "B", Floor = 2, Capacity = 8, HasProjector = false, IsActive = true });
        Rooms.Add(new Room { Id = 4, Name = "Lab 204", BuildingCode = "B", Floor = 2, Capacity = 24, HasProjector = true, IsActive = true });
        Rooms.Add(new Room { Id = 5, Name = "Sala Remontowana", BuildingCode = "C", Floor = 1, Capacity = 30, HasProjector = false, IsActive = false });

       
        Reservations.Add(new Reservation { Id = 1, RoomId = 1, OrganizerName = "Jan Nowak", Topic = "Szkolenie C#", Date = new DateTime(2026, 5, 10), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(15, 0, 0), Status = "confirmed" });
        Reservations.Add(new Reservation { Id = 2, RoomId = 2, OrganizerName = "Anna Kowalska", Topic = "Wykład Otwarty", Date = new DateTime(2026, 5, 10), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 30, 0), Status = "confirmed" });
    }
}