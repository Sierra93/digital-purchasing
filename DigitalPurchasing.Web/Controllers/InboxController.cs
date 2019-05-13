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

        public InboxController(
            IReceivedEmailService receivedEmailService)
        {
            _receivedEmailService = receivedEmailService;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Data(VueTableRequest request)
        {
            var result = _receivedEmailService.GetData(User.CompanyId(), request.Page, request.PerPage, request.SortField, request.SortAsc, request.Search);
            var nextUrl = Url.Action("Data", "Inbox", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "Inbox", request.PrevPageRequest(), Request.Scheme);
            //var data = result.Data.Adapt<List<NomenclatureIndexDataItemEdit>>();
            //foreach (var d in data)
            //{
            //    d.EditUrl = Url.Action("Edit", new { id = d.Id });
            //}
            return Json(new VueTableResponse<InboxIndexDataItem, VueTableRequest>(result.Data, request, result.Total, nextUrl, prevUrl));
        }
    }
}
