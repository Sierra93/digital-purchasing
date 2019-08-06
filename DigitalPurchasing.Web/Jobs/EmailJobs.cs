using System;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Emails;

namespace DigitalPurchasing.Web.Jobs
{
    public class EmailJobs
    {
        private readonly IRobotEmailService _robotEmail;
        private readonly IEmailService _emailService;

        public EmailJobs(
            IRobotEmailService robotEmail,
            IEmailService emailService)
        {
            _robotEmail = robotEmail;
            _emailService = emailService;
        }

        public Task CheckRobotEmails()
            => _robotEmail.CheckRobotEmails();

        public async Task SendPriceReductionEmail(
            Guid ownerId,
            string attachment,
            SupplierContactPersonVm supplierContactPerson,
            UserInfoDto userInfo,
            DateTime until)
            => await _emailService.SendPriceReductionEmail(
                ownerId,
                attachment,
                supplierContactPerson,
                userInfo,
                until);
    }
}
