public class DriverDto
{
    public string FullName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }

    // Add this
    public bool IsActive { get; set; } = true;
}
