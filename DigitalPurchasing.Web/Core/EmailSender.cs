using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace DigitalPurchasing.Web.Core
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.FromResult(true);
        }
    }
}
