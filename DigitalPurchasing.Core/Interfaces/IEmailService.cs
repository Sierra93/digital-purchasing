using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
