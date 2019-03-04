using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Emails;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace DigitalPurchasing.Services
{
    public class RobotEmailService : IRobotEmailService
    {
        private readonly IEnumerable<IEmailProcessor> _emailProcessors;
        private readonly IReceivedEmailService _receivedEmails;

        public RobotEmailService(
            IEnumerable<IEmailProcessor> emailProcessors,
            IReceivedEmailService receivedEmails)
        {
            _emailProcessors = emailProcessors;
            _receivedEmails = receivedEmails;
        }

        public void CheckAppEmail()
        {
            using (var client = new ImapClient(new NullProtocolLogger()))
            {
                client.Connect("imap.yandex.ru", 993, true);
                client.Authenticate("app@digitalpurchasing.com", "Ar7jXeiedFPNYmUNqUZbphPW");

                client.Inbox.Open(FolderAccess.ReadOnly);

                var query = SearchQuery.DeliveredAfter(DateTime.Now.AddHours(-12));

                foreach (var uniqueId in client.Inbox.Search(query))
                {
                    var alreadyProcessed = _receivedEmails.IsProcessed(uniqueId.Id);
                    if (alreadyProcessed) continue;

                    var message = client.Inbox.GetMessage(uniqueId);
                    var subject = message.Subject;
                    var body = message.GetTextBody(TextFormat.Text);
                    var files = new List<string>();
                    var fromEmail = message.From.Mailboxes.First().Address;
                    
                    foreach (var attachment in message.Attachments)
                    {
                        if (attachment is MessagePart) continue;

                        var part = (MimePart) attachment;
                        var filename = part.FileName;
                        var tempPath = Path.GetTempPath();
                        var ext = Path.GetExtension(filename);
                        var path = Path.Combine(tempPath, $"{Guid.NewGuid():N}{ext}");

                        using (var stream = File.Create(path))
                        {
                            part.Content.DecodeTo(stream);
                        }
                        files.Add(path);
                    }

                    var isProcessed = _emailProcessors.Any(q => q.Process(fromEmail, subject, body, files));
                    if (isProcessed)
                    {
                        _receivedEmails.MarkProcessed(uniqueId.Id, true);
                    }
                    else
                    {
                        // try to detect
                        // todo: resend email
                    }
                }
            }
        }
    }

    public class RFQEmailProcessor : IEmailProcessor
    {
        private readonly IQuotationRequestService _quotationRequestService;
        private readonly ICompetitionListService _competitionListService;
        private readonly ISupplierService _supplierService;
        private readonly ISupplierOfferService _supplierOfferService;
        private readonly IEmailService _emailService;
        private readonly ICompanyService _companyService;
        private readonly LinkGenerator _linkGenerator;
        private readonly AppSettings _settings;

        private readonly List<string> _supportedFormats = new List<string>
        {
            ".xlsx"
        };

        public RFQEmailProcessor(
            IConfiguration configuration,
            IQuotationRequestService quotationRequestService,
            ICompetitionListService competitionListService,
            ISupplierService supplierService,
            ISupplierOfferService supplierOfferService,
            IEmailService emailService,
            ICompanyService companyService,
            LinkGenerator linkGenerator)
        {
            _settings = configuration.GetSection(Consts.Settings.AppPath).Get<AppSettings>();
            _quotationRequestService = quotationRequestService;
            _supplierService = supplierService;
            _competitionListService = competitionListService;
            _supplierOfferService = supplierOfferService;
            _emailService = emailService;
            _companyService = companyService;
            _linkGenerator = linkGenerator;
        }

        public bool Process(string fromEmail, string subject, string body, IList<string> attachments)
        {
            if (string.IsNullOrEmpty(subject)) return false;

            if (subject.Contains("[RFQ") && attachments.Any(IsSupportedFile))
            {
                var uidStartIndex = subject.IndexOf("[RFQ", StringComparison.Ordinal);
                var uidEndEnd = subject.IndexOf(']', uidStartIndex);
                var uid = subject.Substring(uidStartIndex+1, uidEndEnd-uidStartIndex-1);

                // qr id
                var qrId = _quotationRequestService.UidToQuotationRequest(uid);
                var qr = _quotationRequestService.GetById(qrId);

                // detect supplier by contact email
                var supplierId = _supplierService.GetSupplierByEmail(fromEmail);

                // get owner email
                var email = _companyService.GetContactEmailByOwner(qr.OwnerId);

                // upload supplier offer
                if (qrId != Guid.Empty && supplierId != Guid.Empty)
                {
                    var clId = _competitionListService.GetIdByQR(qrId, true);

                    foreach (var attachment in attachments.Where(IsSupportedFile))
                    {
                        var createOfferResult = _supplierOfferService.CreateFromFile(clId, attachment);
                        if (!createOfferResult.IsSuccess) // unable parse excel file
                        {
                            // send email with attachment?
                            return true;
                        }

                        // set supplier name
                        var soId = createOfferResult.Id;
                        var supplier = _supplierService.GetById(supplierId, true);
                        _supplierOfferService.UpdateSupplierName(soId, supplier.Name, supplier.Id, true);

                        // detect columns
                        var columns = _supplierOfferService.GetColumnsData(soId, true);
                        _supplierOfferService.UpdateStatus(soId, SupplierOfferStatus.MatchColumns, true);

                        var allColumns = !string.IsNullOrEmpty(columns.Name) &&
                                         !string.IsNullOrEmpty(columns.Uom) &&
                                         !string.IsNullOrEmpty(columns.Price) &&
                                         !string.IsNullOrEmpty(columns.Qty);

                        var allMatched = true;

                        if (allColumns)
                        {
                            _supplierOfferService.SaveColumns(soId, columns, true);
                            // raw items + match
                            _supplierOfferService.GenerateRawItems(soId, true);
                            _supplierOfferService.UpdateStatus(soId, SupplierOfferStatus.MatchItems, true);

                            allMatched = _supplierOfferService.IsAllMatched(soId);
                        }

                        if (!allColumns || !allMatched)
                        {
                            PartiallyProcessedEmail(email, qr.PublicId, qr.Id);
                        }

                        return true;
                    }
                }

                return false; 
            }

            return false;
        }

        private bool IsSupportedFile(string path)
        {
            var ext = Path.GetExtension(path);
            return _supportedFormats.Contains(ext);
        }

        private void PartiallyProcessedEmail(string toEmail, int publicId, Guid id)
        {
            var url = _linkGenerator.GetPathByAction("View", "QuotationRequest", new {id = id });
            var fullUrl = $"{_settings.DefaultDomain}{url}";
            Task.Run(() => _emailService.SendSOPartiallyProcessedEmail(toEmail, publicId, fullUrl));
        }
    }
}
