// Models/Reservation.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace BusReservation.API.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        // FK → Schedule
        [Required]
        public int ScheduleId { get; set; }

        // Navigation
        public Schedule Schedule { get; set; } = default!; // non-null after EF loads it

        // Passenger info
        [Required, MaxLength(100)]
        public string PassengerName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string PassengerPhone { get; set; } = string.Empty;

        public string? PassengerEmail { get; set; }   // <— NEW

        // Seat details
        [MaxLength(10)]
        public string? SeatNumber { get; set; }

        // Status
        public bool IsConfirmed { get; set; } = false;

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedBy { get; set; } // UserId of company/agent

    }
}
