using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage,
            IReadOnlyList<string> attachments = null);

        Task SendFromRobotAsync(string toEmail, string subject, string htmlMessage,
            IReadOnlyList<string> attachments = null);
    }
}
