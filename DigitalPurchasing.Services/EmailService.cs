using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Services
{
    public class EmailService : IEmailService
    {
        private const string AppName = "DigitalPurchasing.com";
        private const string RobotEmail = "app@digitalpurchasing.com";
        private const string DefaultEmail = "hello@digitalpurchasing.com";

        public Task SendFromRobotAsync(string toEmail, string subject, string htmlMessage, IReadOnlyList<string> attachments = null)
        {
            var mailMessage = CreateMailMessage(
                toEmail,
                RobotEmail,
                $"{AppName} Robot",
                RobotEmail,
                $"{AppName} Robot",
                subject,
                htmlMessage);

            var client = CreateClient();
            client.Send(mailMessage);
            return Task.CompletedTask;
        }

        public Task SendEmailAsync(string toEmail, string subject, string htmlMessage, IReadOnlyList<string> attachments = null)
        {
            var mailMessage = CreateMailMessage(
                toEmail,
                DefaultEmail,
                $"{AppName}",
                DefaultEmail,
                $"{AppName}",
                subject,
                htmlMessage);

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
            string subject,
            string htmlMessage,
            IReadOnlyList<string> attachments = null)
        {
            var mailMessage = new MailMessage
            {
                Sender = new MailAddress(fromEmail, fromName),
                From = new MailAddress(fromEmail, fromName)
            };

            mailMessage.ReplyToList.Add(new MailAddress(replyToEmail, replyToName));

            mailMessage.To.Add(toEmail);
            mailMessage.Body = htmlMessage;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;

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
