namespace BusReservation.API.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }

        // NEW: this matches your controller usage
        public bool IsActive { get; set; } = true;
    }
}
