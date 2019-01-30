using System;
using System.IO;
using System.Threading.Tasks;
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
    }
}
