using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels.Inbox;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class InboxController : Controller
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult View(Guid id)
        {
            var soEmail = _receivedEmailService.GetSoEmail(id);

            if (soEmail == null) return NotFound();

            var qr = _quotationRequestService.GetById(soEmail.QuotationRequestId);
            var supplierId = _supplierService.GetSupplierByEmail(qr.OwnerId, soEmail.FromEmail);
            var supplier = _supplierService.GetById(supplierId, true);

            return View(new InboxViewVm
            {
                SupplierName = supplier?.Name,
                EmailBody = soEmail.Body,
                EmailDate = soEmail.MessageDate,
                EmailSubject = soEmail.Subject,
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
