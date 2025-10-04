// File: Dtos/ScheduleDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace BusReservation.API.Dtos
{
    public class ScheduleDto
    {
        [Required]
        public int BusRouteId { get; set; }

        [Required]
        public int BusId { get; set; }

        // Nullable if driver is optional
        public int? DriverId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }
    }
}
