using Application.Options;
using Application.Services.Abstraction;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace UserIdentityInfrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailOptions _options;

        public SmtpEmailService(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                Credentials = new NetworkCredential(_options.Email, _options.Password),
                EnableSsl = true
            };

            using var message = new MailMessage(_options.Email, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await client.SendMailAsync(message, cancellationToken);
        }
    }
}
