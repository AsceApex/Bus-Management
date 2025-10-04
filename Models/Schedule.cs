// Models/Schedule.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace BusReservation.API.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        // FKs
        public int BusRouteId { get; set; }
        public int BusId { get; set; }
        public int? DriverId { get; set; }

        // Navigations
        public BusRoute? BusRoute { get; set; }
        public Bus? Bus { get; set; }
        public Driver? Driver { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
