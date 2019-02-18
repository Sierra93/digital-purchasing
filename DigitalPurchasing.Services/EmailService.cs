using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Services
{
    public class EmailService : IEmailService
    {
        private const string AppName = "DigitalPurchasing.com";
        private const string SenderEmail = "donotreply@digitalpurchasing.com";
        private const string RobotEmail = "app@digitalpurchasing.com";
        private const string DefaultEmail = "hello@digitalpurchasing.com";

        public Task SendFromRobotAsync(string toEmail, string subject, string htmlMessage, IReadOnlyList<string> attachments = null)
        {
            var mailMessage = CreateMailMessage(
                toEmail,
                RobotEmail, $"{AppName} Robot", // from
                RobotEmail, $"{AppName} Robot", // reply to
                SenderEmail, $"{AppName}",      // sender
                subject,
                htmlMessage, attachments);

            var client = CreateClient();
            client.Send(mailMessage);
            return Task.CompletedTask;
        }

        public Task SendEmailAsync(string toEmail, string subject, string htmlMessage, IReadOnlyList<string> attachments = null)
        {
            var mailMessage = CreateMailMessage(
                toEmail,
                DefaultEmail, $"{AppName}", // from
                DefaultEmail, $"{AppName}", // reply to
                SenderEmail, $"{AppName}",  // sender
                subject,
                htmlMessage, attachments);

            var client = CreateClient();
            client.Send(mailMessage);
            return Task.CompletedTask;
        }

        private SmtpClient CreateClient()
        {
            var client = new SmtpClient("smtp.mandrillapp.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("PurchasingTool", "qbm_XHdv5CerLGRZJpYWfQ")
            };
            return client;
        }

        private MailMessage CreateMailMessage(
            string toEmail,
            string fromEmail,
            string fromName,
            string replyToEmail,
            string replyToName,
            string senderEmail,
            string senderName,
            string subject,
            string htmlMessage,
            IReadOnlyList<string> attachments = null)
        {
            var mailMessage = new MailMessage
            {
                Sender = new MailAddress(senderEmail, senderName),
                From = new MailAddress(fromEmail, fromName)
            };

            mailMessage.ReplyToList.Add(new MailAddress(replyToEmail, replyToName));

            mailMessage.To.Add(toEmail);
            mailMessage.Body = htmlMessage;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.HeadersEncoding = Encoding.UTF8;

            if (attachments != null && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    mailMessage.Attachments.Add(new Attachment(attachment));
                }
            }

            return mailMessage;
        }
    }
}
