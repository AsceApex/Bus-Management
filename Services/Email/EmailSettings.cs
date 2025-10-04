// Services/Email/EmailSettings.cs
namespace BusReservation.API.Services
{
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromName { get; set; } = "Bus Reservation";
        public string FromEmail { get; set; } = "no-reply@example.com";
        public bool UseSsl { get; set; } = false; // true = SSL on connect; false = STARTTLS when available
    }
}