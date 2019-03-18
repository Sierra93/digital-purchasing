using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IRobotEmailService
    {
        void CheckRobotEmails();
    }

    public interface IEmailProcessor
    {
        bool Process(string fromEmail, string subject, string body, IList<string> attachments);
    }
}
