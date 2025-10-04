// Services/Email/MailKitEmailSender.cs
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
// using BusReservation.API.Templates;

namespace BusReservation.API.Services
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly EmailSettings _cfg;

        public MailKitEmailSender(IOptions<EmailSettings> options)
        {
            _cfg = options.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_cfg.FromName, _cfg.FromEmail));
            msg.To.Add(MailboxAddress.Parse(toEmail));
            msg.Subject = subject;
            msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();

            var sslOption = _cfg.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;

            await client.ConnectAsync(_cfg.Host, _cfg.Port, sslOption, ct);

            // Only authenticate if credentials are provided
            if (!string.IsNullOrWhiteSpace(_cfg.User))
            {
                await client.AuthenticateAsync(_cfg.User, _cfg.Password, ct);
            }

            await client.SendAsync(msg, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
