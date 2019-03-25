using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IRobotEmailService
    {
        Task CheckRobotEmails();
    }

    public interface IEmailProcessor
    {
        Task<bool> Process(string fromEmail, string subject, string body, IList<string> attachments);
    }
}
