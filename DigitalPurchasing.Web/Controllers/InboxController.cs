using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class InboxController : Controller
    {
        private readonly IReceivedEmailService _receivedEmailService;

        public class InboxTableRequest: VueTableRequest
        {
            public bool UnhandledSupplierOffersOnly { get; set; }
        }

        public InboxController(
            IReceivedEmailService receivedEmailService)
        {
            _receivedEmailService = receivedEmailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult View(Guid id)
        {
            return View();
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
