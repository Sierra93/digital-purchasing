using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Web.Jobs
{
    public class EmailJobs
    {
        private readonly IRobotEmailService _robotEmail;

        public EmailJobs(IRobotEmailService robotEmail)
            => _robotEmail = robotEmail;

        public void CheckAppEmail()
            => _robotEmail.CheckAppEmail();
    }
}