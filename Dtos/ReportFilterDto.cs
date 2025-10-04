namespace BusReservation.API.Dtos
{
    public class ReportFilterDto
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? RouteId { get; set; }
        public int? BusId { get; set; }
        public int? ScheduleId { get; set; }
        public string? CreatedBy { get; set; } // optional: company/agent user id
    }
}
