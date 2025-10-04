// Dtos/RouteDto.cs
namespace BusReservation.API.Dtos;

public class RouteDto
{
    public string StartLocation { get; set; } = string.Empty;
    public string EndLocation { get; set; } = string.Empty;
    public double DistanceInKm { get; set; }
    public TimeSpan EstimatedTime { get; set; }
}
