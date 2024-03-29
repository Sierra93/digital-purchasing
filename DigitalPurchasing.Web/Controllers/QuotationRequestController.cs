using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.ExcelReader;
using DigitalPurchasing.Models;
using DigitalPurchasing.Models.Identity;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.Jobs;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.QuotationRequest;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Controllers
{
    public class QuotationRequestController : BaseController
    {
        public class SentRequestsModel
        {
            public class ItemSupplier
            {
                public Guid ItemId { get; set; }
                public Guid SupplierId { get; set; }
            }

            [JsonProperty("id")]
            public Guid QuotationRequestId { get; set; }
            public List<Guid> Suppliers { get; set; }
            public List<ItemSupplier> ItemSuppliers { get; set; }
        }

        private readonly IQuotationRequestService _quotationRequestService;
        private readonly ICompetitionListService _competitionListService;
        private readonly IRootService _rootService;
        private readonly UserManager<User> _userManager;

        public QuotationRequestController(
            IQuotationRequestService quotationRequestService,
            ICompetitionListService competitionListService,
            IRootService rootService,
            UserManager<User> userManager)
        {
            _quotationRequestService = quotationRequestService;
            _competitionListService = competitionListService;
            _rootService = rootService;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var nextUrl = Url.Action("Data", "QuotationRequest", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "QuotationRequest", request.PrevPageRequest(), Request.Scheme);

            var result = _quotationRequestService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);

            var data = result.Data
                .Adapt<List<QuotationRequestIndexDataItemVm>>()
                .Select(q =>
                {
                    q.EditUrl = Url.Action(nameof(View), new { id = q.Id });
                    q.CreatedOn = User.ToLocalTime(q.CreatedOn);
                    return q;
                })
                .ToList();

            return Json(new VueTableResponse<QuotationRequestIndexDataItemVm, VueTableRequest>(data, request, result.Total, nextUrl, prevUrl));
        }

        public async Task<IActionResult> Create([FromQuery]Guid prId)
        {
            var id = await _quotationRequestService.GetQuotationRequestId(prId);
            if (id == Guid.Empty) return NotFound();
            return RedirectToAction(nameof(View), new { id });
        }

        public IActionResult View(Guid id)
        {
            var model = _quotationRequestService.GetById(id);
            return View(model);
        }

        public IActionResult ViewDData([FromQuery]Guid qrId)
        {
            var model = _quotationRequestService.GetViewData(qrId);
            return Ok(model);
        }

        public IActionResult Download([FromQuery]Guid qrId)
        {
            var qr = _quotationRequestService.GetById(qrId);
            var bytes = _quotationRequestService.GenerateExcelByCategory(qrId);
            var filename = $"RFQ_{qr.CreatedOn:yyyyMMdd}_{qr.PublicId}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        [HttpPost]
        public IActionResult Delete([FromBody]DeleteVm vm)
        {
            var result = _quotationRequestService.Delete(vm.Id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SentRequests([FromBody] SentRequestsModel model)
        {
            await _quotationRequestService.SendRequests(
                User.Id(),
                model.QuotationRequestId,
                model.Suppliers,
                model.ItemSuppliers
                    .Select(q => (SupplierId: q.SupplierId, ItemId: q.ItemId))
                    .ToList());
            var sentRequests = _quotationRequestService.GetSentRequests(model.QuotationRequestId);

            var root = await _rootService.GetByQR(model.QuotationRequestId);
            var competitionListId = root.CompetitionListId
                                    ?? await _competitionListService.GetIdByQR(model.QuotationRequestId, true);
            
            var user = await _userManager.GetUserAsync(User);

            // todo: set pr data

            var isAutomaticCloseDateSet = await _competitionListService.IsAutomaticCloseDateSet(competitionListId);
            if (!isAutomaticCloseDateSet)
            {
                await _competitionListService.SetAutomaticCloseInHours(competitionListId, user.AutoCloseCLHours);
                Hangfire.BackgroundJob.Schedule<CompetitionListJobs>(q => q.Close(competitionListId),
                    TimeSpan.FromHours(user.AutoCloseCLHours));

                if (user.RoundsCount > 0)
                {
                    var now = DateTime.UtcNow;
                    for (var i = 0; i < user.RoundsCount; i++)
                    {
                        var delay = now
                            .AddHours(user.QuotationRequestResponseHours)
                            .AddHours(user.PriceReductionResponseHours * i);
                        var round = i + 1;
                        Hangfire.BackgroundJob.Schedule<PriceReductionJobs>(q => q.SendPriceReductionRequests(competitionListId, user.Id, user.CompanyId, round), delay);
                    }
                }
            }
            
            return Ok(sentRequests);
        }
    }
}
