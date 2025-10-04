using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusReservation.API.Models;

public class BusRoute
{
    public int Id { get; set; }
    public string StartLocation { get; set; } = string.Empty;
    public string EndLocation { get; set; } = string.Empty;
    public double DistanceInKm { get; set; }
    public TimeSpan EstimatedTime { get; set; }

}
