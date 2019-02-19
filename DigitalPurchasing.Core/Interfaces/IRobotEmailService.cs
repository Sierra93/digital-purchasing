using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IRobotEmailService
    {
        void CheckAppEmail();
    }

    public interface IEmailProcessor
    {
        void Process();
    }
}
