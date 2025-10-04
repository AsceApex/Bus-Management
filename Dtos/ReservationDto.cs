public class ReservationDto
{
    public int ScheduleId { get; set; }
    public required string PassengerName { get; set; }
    public required string PassengerPhone { get; set; }
    public required string SeatNumber { get; set; }

    public string? PassengerEmail { get; set; }   // <— NEW

    public string? CreatedBy { get; set; }
}
