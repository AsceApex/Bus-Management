public static class ReservationTemplates
{
    public static string Created(string passenger, string seat, DateTime dep, string from, string to, string bus)
        => $@"<h2>Reservation Created</h2>
<p>Dear {passenger},</p>
<p>Your seat <strong>{seat}</strong> has been reserved.</p>
<ul>
  <li>Route: {from} → {to}</li>
  <li>Departure: {dep:dd MMM yyyy, HH:mm}</li>
  <li>Bus: {bus}</li>
</ul>
<p>We will notify you after confirmation.</p>";

    public static string Confirmed(string passenger, string seat, DateTime dep, string from, string to, string bus)
        => $@"<h2>Reservation Confirmed ✅</h2>
<p>Dear {passenger},</p>
<p>Your seat <strong>{seat}</strong> is now <strong>CONFIRMED</strong>.</p>
<ul>
  <li>Route: {from} → {to}</li>
  <li>Departure: {dep:dd MMM yyyy, HH:mm}</li>
  <li>Bus: {bus}</li>
</ul>
<p>Thank you for choosing us!</p>";
}
