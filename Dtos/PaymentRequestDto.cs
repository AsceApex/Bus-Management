namespace BusReservation.API.Dtos
{
    public class PaymentRequestDto
    {
        // Amount must be in paise (e.g., ₹100.00 = 10000)
        public int Amount { get; set; }

        // Default currency INR
        public string Currency { get; set; } = "INR";

        // ReceiptId should be optional, so mark nullable
        public string? ReceiptId { get; set; }
    }
}
