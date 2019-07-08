using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using Mandrill;
using Mandrill.Model;

namespace DigitalPurchasing.Services
{
    public class EmailService : IEmailService
    {
        private readonly IMandrillMessagesApi _messages;

        private const string AppName = "DigitalPurchasing.com";
        private const string NotificationsEmail = "notifications@digitalpurchasing.com"; // only for sending (from)
        private string RobotEmail(Guid companyId) => $"robot+{companyId:N}@digitalpurchasing.com";
        private const string DefaultEmail = "hello@digitalpurchasing.com";

        public EmailService(IMandrillMessagesApi messages)
            => _messages = messages;

        public async Task SendFromRobotAsync(Guid companyId, string toEmail, string subject, string htmlMessage, IReadOnlyList<string> attachments = null)
        {
            var mailMessage = CreateMailMessage(
                toEmail,
                RobotEmail(companyId), $"{AppName} Robot", // from
                RobotEmail(companyId), $"{AppName} Robot", // reply to
                subject,
                htmlMessage, attachments);

            await _messages.SendAsync(mailMessage);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage, IReadOnlyList<string> attachments = null)
        {
            var mailMessage = CreateMailMessage(
                toEmail,
                NotificationsEmail, $"{AppName}", // from
                DefaultEmail, $"{AppName}", // reply to
                subject,
                htmlMessage, attachments);

            await _messages.SendAsync(mailMessage);
        }

        private MandrillMessage CreateMailMessage(
            string toEmail,
            string fromEmail,
            string fromName,
            string replyToEmail,
            string replyToName,
            string subject,
            string htmlMessage,
            IReadOnlyList<string> attachments = null)
        {
            var message = new MandrillMessage();
            message.AddTo(toEmail);
            message.FromEmail = fromEmail;
            message.FromName = fromName;
            message.ReplyTo = $"{replyToName} <{replyToEmail}>";
            message.Subject = subject;
            message.Html = htmlMessage;

            if (attachments != null && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    var filename = Path.GetFileName(attachment);
                    var type = MimeTypes.GetMimeType(filename);
                    var bytes = File.ReadAllBytes(attachment);
                    message.Attachments.Add(new MandrillAttachment(type, filename, bytes));
                }
            }

            return message;
        }
    }
}
