using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Emails;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace DigitalPurchasing.Services
{
    public class RobotEmailService : IRobotEmailService
    {
        private readonly IEnumerable<IEmailProcessor> _emailProcessors;
        private readonly IReceivedEmailService _receivedEmails;
        private readonly IQuotationRequestService _quotationRequestService;

        public RobotEmailService(
            IEnumerable<IEmailProcessor> emailProcessors,
            IReceivedEmailService receivedEmails,
            IQuotationRequestService quotationRequestService)
        {
            _emailProcessors = emailProcessors;
            _receivedEmails = receivedEmails;
            _quotationRequestService = quotationRequestService;
        }

        public async Task CheckRobotEmails()
        {
            using (var client = new ImapClient(new NullProtocolLogger()))
            {
                client.Connect("imap.yandex.ru", 993, true);
                client.Authenticate("app@digitalpurchasing.com", "Ar7jXeiedFPNYmUNqUZbphPW");

                client.Inbox.Open(FolderAccess.ReadOnly);

                var query = SearchQuery.DeliveredAfter(DateTime.Now.AddHours(-12));

                foreach (var uniqueId in client.Inbox.Search(query))
                {
                    if (!_receivedEmails.IsProcessed(uniqueId.Id))
                    {
                        var message = client.Inbox.GetMessage(uniqueId);
                        Guid? emailId = null;

                        if (RFQEmailProcessor.IsSupplierOfferEmail(message))
                        {
                            emailId = SaveSoEmail(uniqueId, message);
                        }
                        else
                        {
                            // todo
                        }

                        if (emailId.HasValue)
                        {
                            var processTasks = _emailProcessors.Select(q => q.Process(emailId.Value));
                            var processResults = await Task.WhenAll(processTasks);

                            var isProcessed = processResults.Any(q => q);
                            if (isProcessed)
                            {
                                _receivedEmails.MarkProcessed(uniqueId.Id);
                            }
                            else
                            {
                                _receivedEmails.IncProcessingTries(uniqueId.Id);

                                // try to detect
                                // todo: resend email
                            }
                        }
                    }
                }
            }
        }

        private Guid? SaveSoEmail(UniqueId messageId, MimeMessage message)
        {
            string rfqUid = RFQEmailProcessor.GetRfqEmailUid(message);
            var qrId = _quotationRequestService.UidToQuotationRequest(rfqUid);
            if (qrId.HasValue)
            {
                var body = message.GetTextBody(TextFormat.Html);
                var fromEmail = message.From.Mailboxes.First().Address;

                return _receivedEmails.SaveSoEmail(messageId.Id, qrId.Value, message.Subject, body, fromEmail, message.Date,
                    message.Attachments.Where(a => !(a is MessagePart)).Select(a =>
                    {
                        var part = (MimePart)a;
                        using (var ms = new MemoryStream())
                        {
                            part.Content.DecodeTo(ms);
                            return (part.FileName, part.ContentType.MimeType, ms.ToArray());
                        }
                    }).ToList());
            }

            return null;
        }
    }

    public class RFQEmailProcessor : IEmailProcessor
    {
        private readonly ILogger _logger;

        private readonly IQuotationRequestService _quotationRequestService;
        private readonly ICompetitionListService _competitionListService;
        private readonly ISupplierService _supplierService;
        private readonly ISupplierOfferService _supplierOfferService;
        private readonly IEmailService _emailService;
        private readonly ICompanyService _companyService;
        private readonly IRootService _rootService;
        private readonly LinkGenerator _linkGenerator;
        private readonly AppSettings _settings;
        private readonly IReceivedEmailService _receivedEmails;

        private static List<string> _supportedFormats = new List<string>
        {
            ".xlsx", ".xls"
        };

        public RFQEmailProcessor(
            ILogger<RFQEmailProcessor> logger,
            IConfiguration configuration,
            IQuotationRequestService quotationRequestService,
            ICompetitionListService competitionListService,
            ISupplierService supplierService,
            ISupplierOfferService supplierOfferService,
            IEmailService emailService,
            ICompanyService companyService,
            LinkGenerator linkGenerator,
            IRootService rootService,
            IReceivedEmailService receivedEmails)
        {
            _logger = logger;
            _settings = configuration.GetSection(Consts.Settings.AppPath).Get<AppSettings>();
            _quotationRequestService = quotationRequestService;
            _supplierService = supplierService;
            _competitionListService = competitionListService;
            _supplierOfferService = supplierOfferService;
            _emailService = emailService;
            _companyService = companyService;
            _linkGenerator = linkGenerator;
            _rootService = rootService;
            _receivedEmails = receivedEmails;
        }

        internal static bool IsSupplierOfferEmail(MimeMessage message)
        {
            Func<MimePart, bool> isSupportedFile = (part) =>
            {
                var ext = Path.GetExtension(part.FileName);
                return _supportedFormats.Contains(ext);
            };

            return !string.IsNullOrEmpty(message.Subject) &&
                message.Subject.Contains("[RFQ");
        }

        internal static string GetRfqEmailUid(MimeMessage message)
        {
            var uidStartIndex = message.Subject.IndexOf("[RFQ", StringComparison.Ordinal);
            var uidEndEnd = message.Subject.IndexOf(']', uidStartIndex);
            return message.Subject.Substring(uidStartIndex + 1, uidEndEnd - uidStartIndex - 1);
        }

        public async Task<bool> Process(Guid emailId)
        {
            var soEmail = _receivedEmails.GetSoEmail(emailId);

            if (soEmail != null)
            {
                var qr = _quotationRequestService.GetById(soEmail.QuotationRequestId, true);
                if (qr != null)
                {
                    var ownerEmail = _companyService.GetContactEmailByOwner(qr.OwnerId);

                    var soHandled = false;

                    // detect supplier by contact email
                    var supplierId = _supplierService.GetSupplierByEmail(qr.OwnerId, soEmail.FromEmail);

                    // upload supplier offer
                    if (supplierId != Guid.Empty)
                    {
                        var clId = await _competitionListService.GetIdByQR(soEmail.QuotationRequestId, true);

                        foreach (var attachment in soEmail.Attachments.Where(a => IsSupportedFile(a.FileName)))
                        {
                            var allColumns = false;
                            var allMatched = false;

                            var tempFile = Path.GetTempFileName() + Path.GetExtension(attachment.FileName);
                            try
                            {
                                File.WriteAllBytes(tempFile, attachment.Bytes);

                                var createOfferResult = await _supplierOfferService.CreateFromFile(clId, tempFile);
                                if (createOfferResult.IsSuccess)
                                {
                                    try
                                    {
                                        // set supplier name
                                        var soId = createOfferResult.Id;
                                        var supplier = _supplierService.GetById(supplierId, true);
                                        _supplierOfferService.UpdateSupplierName(soId, supplier.Name, supplier.Id, true);

                                        // detect columns
                                        var columns = _supplierOfferService.GetColumnsData(soId, true);
                                        _supplierOfferService.UpdateStatus(soId, SupplierOfferStatus.MatchColumns, true);

                                        allColumns = !string.IsNullOrEmpty(columns.Name) &&
                                                     !string.IsNullOrEmpty(columns.Uom) &&
                                                     !string.IsNullOrEmpty(columns.Price) &&
                                                     !string.IsNullOrEmpty(columns.Qty);

                                        if (allColumns)
                                        {
                                            _supplierOfferService.SaveColumns(soId, columns, true);
                                            // raw items + match
                                            _supplierOfferService.GenerateRawItems(soId, true);
                                            _supplierOfferService.UpdateStatus(soId, SupplierOfferStatus.MatchItems, true);

                                            allMatched = _supplierOfferService.IsAllMatched(soId);
                                            var rootId = await _rootService.GetIdByQR(soEmail.QuotationRequestId);
                                            await _rootService.SetStatus(rootId, allMatched
                                                ? RootStatus.EverythingMatches
                                                : RootStatus.MatchingRequired);
                                        }

                                        if (!allColumns || !allMatched)
                                        {
                                            PartiallyProcessedEmail(ownerEmail, qr.PublicId, createOfferResult.Id /* SO Id */);
                                        }

                                        soHandled = true;
                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.LogError(e, "Error during processing RFQ email");
                                    }
                                }
                                else
                                {
                                    // unable parse excel file
                                    // todo: send email with attachment?
                                }
                            }
                            finally
                            {
                                TryDeleteFile(tempFile);
                            }
                        }
                    }

                    if (soHandled)
                    {
                        return true;
                    }
                    else
                    {
                        if (soEmail.ProcessingTries == 0)
                        {
                            SendSoNotProcessedNotification(ownerEmail, qr.PublicId, soEmail.Id);
                        }
                    }
                }
            }

            return false;
        }

        private void TryDeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {

            }
        }

        private bool IsSupportedFile(string path)
        {
            var ext = Path.GetExtension(path);
            return _supportedFormats.Contains(ext);
        }

        private void SendSoNotProcessedNotification(string toEmail, int qrPublicId, Guid emailId)
        {
            var url = _linkGenerator.GetPathByAction("View", "Inbox", new { id = emailId });
            var fullUrl = $"{_settings.DefaultDomain}{url}";
            Task.Run(() => _emailService.SendSoNotProcessedEmail(toEmail, qrPublicId, fullUrl));
        }

        private void PartiallyProcessedEmail(string toEmail, int qrPublicId, Guid supplierOfferId)
        {
            var url = _linkGenerator.GetPathByAction("Edit", "SupplierOffer", new { id = supplierOfferId });
            var fullUrl = $"{_settings.DefaultDomain}{url}";
            Task.Run(() => _emailService.SendSOPartiallyProcessedEmail(toEmail, qrPublicId, fullUrl));
        }
    }
}
