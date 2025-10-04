namespace BusReservation.API.Models
{
    public class BusSeat
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public Bus Bus { get; set; } = default!;
        public string SeatNumber { get; set; } = default!; // e.g., "1A", "1B", "10D"
    }
}
