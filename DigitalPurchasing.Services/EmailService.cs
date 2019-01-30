using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return;
        }
    }
}
