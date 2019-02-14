using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage, ReplyTo replyTo = ReplyTo.Hello)
        {
            var client = new SmtpClient("smtp.mandrillapp.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("PurchasingTool", "qbm_XHdv5CerLGRZJpYWfQ")
            };

            var mailMessage = new MailMessage
            {
                Sender = new MailAddress("donotreply@digitalpurchasing.com", "DigitalPurchasing.com"),
                From = new MailAddress("donotreply@digitalpurchasing.com", "DigitalPurchasing.com"),
            };

            var defaultMailAddress = new MailAddress("hello@digitalpurchasing.com", "DigitalPurchasing.com");

            switch (replyTo)
            {
                case ReplyTo.Hello:
                    mailMessage.ReplyToList.Add(defaultMailAddress);
                    break;
                case ReplyTo.App:
                    mailMessage.ReplyToList.Add(new MailAddress("app@digitalpurchasing.com", "DigitalPurchasing.com"));
                    break;
                default:
                    mailMessage.ReplyToList.Add(defaultMailAddress);
                    break;
            }
            
            mailMessage.To.Add(email);
            mailMessage.Body = htmlMessage;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            client.Send(mailMessage);
            return Task.FromResult(true);
        }
    }
}
