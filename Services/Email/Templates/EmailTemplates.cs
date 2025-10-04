// Templates/EmailTemplates.cs
namespace BusReservation.API.Templates
{
    public static class EmailTemplates
    {
        public static string ReservationCreated(string passengerName, string routeName, string seatNumber)
        {
            return $@"
<h2>Reservation Confirmed</h2>
<p>Hi {passengerName},</p>
<p>Your reservation for <strong>{routeName}</strong> is confirmed.</p>
<p>Seat: <strong>{seatNumber}</strong></p>
<p>Thank you for riding with us.</p>";
        }
    }
}
