using System;
using System.Linq;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels.Inbox;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class InboxController : BaseController
    {
        private readonly IReceivedEmailService _receivedEmailService;
        private readonly IQuotationRequestService _quotationRequestService;
        private readonly ISupplierService _supplierService;

        public class InboxTableRequest: VueTableRequest
        {
            public bool UnhandledSupplierOffersOnly { get; set; }
        }

        public InboxController(
            IReceivedEmailService receivedEmailService,
            IQuotationRequestService quotationRequestService,
            ISupplierService supplierService)
        {
            _receivedEmailService = receivedEmailService;
            _quotationRequestService = quotationRequestService;
            _supplierService = supplierService;
        }

        public IActionResult Index() => View();

        public IActionResult View(Guid id)
        {
            var soEmail = _receivedEmailService.GetEmail(id);
            if (soEmail == null) return NotFound();

            var ownerId = User.CompanyId();
            var supplierName = _supplierService.GetSupplierNameByEmail(ownerId, soEmail.FromEmail);

            return View(new InboxViewVm
            {
                SupplierName = string.IsNullOrEmpty(supplierName) ? "Не определен" : supplierName,
                EmailBody = soEmail.Body,
                EmailDate = soEmail.MessageDate,
                EmailSubject = soEmail.Subject,
                EmailFrom = soEmail.FromEmail,
                Attachments = soEmail.Attachments.Select(a => new InboxViewVm.EmailAttachment()
                {
                    FileName = a.FileName,
                    Id = a.Id
                }).ToList()
            });
        }

        public IActionResult Data(InboxTableRequest request)
        {
            var result = _receivedEmailService.GetData(User.CompanyId(), request.UnhandledSupplierOffersOnly,
                request.Page, request.PerPage, request.SortField, request.SortAsc, request.Search);
            var nextUrl = Url.Action("Data", "Inbox", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "Inbox", request.PrevPageRequest(), Request.Scheme);

            foreach (var dataItem in result.Data)
            {
                dataItem.MessageDate = User.ToLocalTime(dataItem.MessageDate.UtcDateTime);
            }

            return Json(new VueTableResponse<InboxIndexDataItem, VueTableRequest>(result.Data, request, result.Total, nextUrl, prevUrl));
        }

        public IActionResult DownloadAttachment(Guid attachmentId)
        {
            var attachment = _receivedEmailService.GetAttachment(attachmentId);

            if (attachment == null)
            {
                return NotFound();
            }

            return File(attachment.Bytes, attachment.ContentType, attachment.FileName);
        }
    }
}
