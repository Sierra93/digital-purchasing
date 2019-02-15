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

            var until = DateTime.UtcNow.AddDays(3).ToRussianStandardTime();

            var model = new RFQEmail
            {
                From =
                {
                    Name = $"{userInfo.LastName} {userInfo.FirstName}",
                    JobTitle = userInfo.JobTitle,
                    Company = userInfo.Company,
                },
                Until = until,
                ToName = supplierContact.LastName
            };

            var htmlResult = await GetHtmlString(model);
            await emailService.SendEmailAsync(supplierContact.Email, subject, htmlResult, new List<string> { attachment });
        }
    }
}
