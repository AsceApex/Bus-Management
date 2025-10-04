namespace BusReservation.API.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public int RouteId { get; set; }
        public int ScheduleId { get; set; }
        public string? UserId { get; set; }
    }
}
