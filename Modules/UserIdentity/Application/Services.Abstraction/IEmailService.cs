using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    }
}
