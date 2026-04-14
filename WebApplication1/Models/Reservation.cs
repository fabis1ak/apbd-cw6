using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Reservation
{
    public int Id { get; set; }
    
    [Required]
    public int RoomId { get; set; }
    
    [Required(ErrorMessage = "Nazwa organizatora jest wymagana.")]
    public string OrganizerName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Temat jest wymagany.")]
    public string Topic { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    public string Status { get; set; } = "planned";
}