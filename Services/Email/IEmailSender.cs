// Services/IEmailSender.cs
using System.Threading;
using System.Threading.Tasks;

namespace BusReservation.API.Services
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);
    }
}
