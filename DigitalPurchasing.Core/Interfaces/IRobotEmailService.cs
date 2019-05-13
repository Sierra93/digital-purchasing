using System;
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
        Task<bool> Process(Guid emailId);
    }
}
