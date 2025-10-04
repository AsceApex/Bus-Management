namespace BusReservation.API.Models;

public class Bus
{
    public int Id { get; set; }
    public string BusNumber { get; set; } = string.Empty;
    public string BusType { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
