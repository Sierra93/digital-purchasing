using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Emails.EmailTemplates;
using RazorLight;

namespace DigitalPurchasing.Emails
{
    public static class EmailServiceExtensions
    {
        private static async Task<string> GetHtmlString(string template, object model)
        {
            var templateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");
            var engine = new RazorLightEngineBuilder()
                .UseFilesystemProject(templateFilePath)
                .UseMemoryCachingProvider()
                .Build();
            var htmlResult = await engine.CompileRenderAsync(template, model);
            return htmlResult;
        }

        private static async Task<string> GetHtmlString<TModel>(TModel model)
        {
            var template = $"{typeof(TModel).Name}.cshtml";
            return await GetHtmlString(template, model);
        }

        public static async Task SendDummyEmail(this IEmailService emailSender, string email)
        {
            var emailAddress = email;
            var emailSubject = "Test email";
            var model = new DummyEmail { Text = "Test email body" };
            var htmlResult = await GetHtmlString(model);
            await emailSender.SendEmailAsync(emailAddress, emailSubject, htmlResult);
        }

        public static async Task SendRFQEmail(this IEmailService emailService,
            QuotationRequestVm quotationRequest,
            UserInfoDto userInfo,
            SupplierContactPersonVm supplierContact,
            string emailUid,
            string attachment)
        {
            var subject = $"[{emailUid}] Запрос коммерческого предложения №{quotationRequest.PublicId}";
            var until = DateTime.UtcNow.AddHours(1).ToRussianStandardTime();
            var toName = supplierContact.ToName();

            var model = new RFQEmail
            {
                From =
                {
                    Name = $"{userInfo.LastName} {userInfo.FirstName}",
                    JobTitle = userInfo.JobTitle,
                    Company = userInfo.Company,
                },
                Until = until,
                ToName = toName
            };

            var htmlResult = await GetHtmlString(model);
            await emailService.SendFromRobotAsync(quotationRequest.OwnerId, supplierContact.Email, subject, htmlResult, new List<string> { attachment });
        }

        public static async Task SendSOPartiallyProcessedEmail(this IEmailService emailService,
            string email,
            int qrPublicId,
            string qrUrl)
        {
            var subject = "Получено новое КП";

            var model = new SOPartiallyProcessedEmail
            {
                QRPublicId = qrPublicId,
                Url = qrUrl
            };

            var htmlResult = await GetHtmlString(model);
            await emailService.SendEmailAsync(email, subject, htmlResult);
        }

        public static async Task SendSoNotProcessedEmail(this IEmailService emailService,
            string email,
            int qrPublicId,
            string soEmailUrl)
        {
            var subject = "Вам пришло новое КП, но мы не смогли его обработать";

            var model = new SoNotProcessedEmail
            {
                QrPublicId = qrPublicId,
                ViewSoEmailUrl = soEmailUrl
            };

            var htmlResult = await GetHtmlString(model);
            await emailService.SendEmailAsync(email, subject, htmlResult);
        }

        public static async Task SendEmailConfirmationEmail(this IEmailService emailService, string email, string url)
        {
            var subject = "Подтвердите ваш адрес электронной почты";
            var model = new EmailConfirmationEmail
            {
                Url = url
            };

            var htmlResult = await GetHtmlString(model);
            await emailService.SendEmailAsync(email, subject, htmlResult);
        }

        public static async Task SendPriceReductionEmail(this IEmailService emailService,
            Guid ownerId,
            string emailUid,
            string attachment,
            SupplierContactPersonVm supplierContactPerson,
            UserInfoDto userInfo,
            DateTime until,
            string invoiceData)
        {

            var subject = $"[{emailUid}] Запрос на изменение условий КП/cчета";
            var model = new PriceReductionEmail
            {
                InvoiceData = invoiceData,
                ToName = supplierContactPerson.ToName(),
                Until = until.ToRussianStandardTime(),
                From = new PriceReductionEmail.FromData
                {
                    Company = userInfo.Company,
                    Name = $"{userInfo.LastName} {userInfo.FirstName}",
                    PhoneNumber = userInfo.PhoneNumber
                }
            };

            var htmlMessage = await GetHtmlString(model);
            var attachments = new[] { attachment };
            await emailService.SendFromRobotAsync(ownerId, supplierContactPerson.Email, subject, htmlMessage, attachments);
        }
    }
}
