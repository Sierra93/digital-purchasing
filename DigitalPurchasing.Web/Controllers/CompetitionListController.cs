using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Emails;
using DigitalPurchasing.ExcelReader;
using DigitalPurchasing.Models.Identity;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.Jobs;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.CompetitionList;
using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class CompetitionListController : BaseController
    {
        private readonly ICompetitionListService _competitionListService;
        private readonly ISupplierOfferService _supplierOfferService;
        private readonly ISelectedSupplierService _selectedSupplierService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly ISupplierService _supplierService;
        private readonly IQuotationRequestService _quotationRequestService;
        private readonly IRootService _rootService;
        private readonly UserManager<User> _userManager;

        public CompetitionListController(
            ICompetitionListService competitionListService,
            ISupplierOfferService supplierOfferService,
            ISelectedSupplierService selectedSupplierService,
            IEmailService emailService,
            IUserService userService,
            ISupplierService supplierService,
            IQuotationRequestService quotationRequestService,
            IRootService rootService,
            UserManager<User> userManager)
        {
            _competitionListService = competitionListService;
            _supplierOfferService = supplierOfferService;
            _selectedSupplierService = selectedSupplierService;
            _emailService = emailService;
            _userService = userService;
            _supplierService = supplierService;
            _quotationRequestService = quotationRequestService;
            _rootService = rootService;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult IndexData(VueTableRequest request)
        {
            var nextUrl = Url.Action("IndexData", "CompetitionList", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("IndexData", "CompetitionList", request.PrevPageRequest(), Request.Scheme);

            var result = _competitionListService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);

            var data = result.Data
                .Adapt<List<CompetitionListIndexDataItemVm>>()
                .Select(q => { q.EditUrl = Url.Action(nameof(Edit), new { id = q.Id }); return q; })
                .ToList();

            return Json(new VueTableResponse<CompetitionListIndexDataItemVm, VueTableRequest>(data, request, result.Total, nextUrl, prevUrl));
        }

        public async Task<IActionResult> Create([FromQuery]Guid qrId)
        {
            var id = await _competitionListService.GetIdByQR(qrId, false);
            if (id == Guid.Empty) return NotFound();
            return RedirectToAction(nameof(Edit), new { id });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var cl = _competitionListService.GetById(id, false);
            if (cl == null) return NotFound();

            var vm = new CompetitionListEditVm
            {
                CompetitionList = cl
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromRoute]Guid id, IFormFile file)
        {
            if (file == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var fileName = file.FileName;
            var fileExt = Path.GetExtension(fileName);
            var filePath = Path.GetTempFileName()+fileExt;

            using (var output = System.IO.File.Create(filePath))
                await file.CopyToAsync(output);

            var response = await _supplierOfferService.CreateFromFile(id, filePath);
            if (!response.IsSuccess)
            {
                TempData["Message"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Edit), "SupplierOffer", new { id = response.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Report(Guid reportId)
        {
            var reportData = await _selectedSupplierService.GetReport(reportId);
            var report = new ExcelSSR(reportData);
            var fileBytes = report.Build();
            var fileName = $"{reportData.CLCreatedOn:yyyyMMdd}_КЛ {reportData.CLNumber}_Отчет о выборе поставщика.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public IActionResult PriceReductionDownload([FromRoute] Guid id, [FromQuery] Guid supplierId,
            [FromBody] SendPriceReductionRequestsVm model)
        {
            var cl = _competitionListService.GetById(id, false);//todo: use supplierId
            if (cl == null) return NotFound();

            var offers = cl.GroupBySupplier().First(q => q.Value.First().SupplierId == supplierId);
            var lastOffer = offers.Value.Last();

            var reportData = CreatePriceReductionData(lastOffer, cl, model); 

            var report = new PriceReductionWriter(reportData);
            var fileBytes = report.Build();
            var fileName = $"{cl.CreatedOn:yyyyMMdd}_КЛ_{cl.PublicId}_{lastOffer.SupplierName}_Запрос_на_изменение_условий.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public IActionResult Delete([FromBody]DeleteVm vm)
        {
            var result = _competitionListService.Delete(vm.Id);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult LoadSOTerms([FromQuery]Guid? soId)
        {
            if (soId.HasValue)
            {
                return Ok(_supplierOfferService.GetTerms(soId.Value));
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult SaveSOTerms([FromBody]SoTermsVm req, [FromQuery]Guid? soId)
        {
            if (req == null || !soId.HasValue) return NotFound();

            _supplierOfferService.SaveTerms(req, soId.Value);
            return Ok();
        }

        public class PriceReductionDataVm
        {
            public class Supplier
            {
                public Guid Id { get; set; }
                public string Name { get; set; }
                public DateTime CreatedOn { get; set; }
                public bool IsChecked { get; set; }
            }

            public class ItemSupplier
            {
                public Guid Id { get; set; }
                public bool IsChecked { get; set; }
                public bool IsEnabled { get; set; }
                public bool IsSent { get; set; }
            }

            public class Item
            {
                public int Position { get; set; }
                public decimal MinPrice { get; set; }
                public decimal TargetPrice => MinPrice > 0 ? Math.Round(MinPrice * ( 1 - Discount/100), 2) : -1;
                public decimal Discount { get; set; }

                /// Supplier offers
                public List<ItemSupplier> Suppliers { get; set; }
                public Guid Id { get; set; }
                public DateTime? SentDate { get; set; }
            }

            public List<Supplier> Suppliers { get; set; }
            public List<Item> Items { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> PriceReductionData([FromRoute]Guid id)
        {
            var cl = _competitionListService.GetById(id, false);

            var offers = cl.GroupBySupplier()
                .Where(q => q.Key.SupplierId.HasValue)
                .Select(q => q.Value.Last())
                .ToList();

            var emails = (await _competitionListService.GetPriceReductionEmailsByCL(id)).ToList();

            var user = await _userService.GetById(User.Id());

            var vm = new PriceReductionDataVm
            {
                Suppliers = offers.OrderBy(q => q.CreatedOn).Select(q => new PriceReductionDataVm.Supplier
                {
                    Id = q.Id,
                    Name = q.SupplierName,
                    CreatedOn = q.CreatedOn,
                    IsChecked = true
                }).ToList(),

                Items = cl.PurchaseRequest.Items.Select(pri =>
                {
                    var itemEmails = emails.Where(q => q.Data.Contains(pri.Id)).ToList();
                    
                    return new PriceReductionDataVm.Item
                    {
                        Id = pri.Id,
                        Position = pri.Position,
                        Discount = user.PRDiscountPercentage,
                        MinPrice = cl.GetMinimalOfferPrice(pri.Id),
                        Suppliers = offers.OrderBy(q => q.CreatedOn).Select(so =>
                        {
                            return new PriceReductionDataVm.ItemSupplier
                            {
                                Id = so.Id,
                                IsChecked = true,
                                IsEnabled = so.Items.Any(soi
                                    => soi.Request.ItemId == pri.Id && soi.Offer.Price > 0),
                                IsSent = itemEmails.Any(q => q.SupplierOfferId == so.Id)
                            };
                        }).ToList(),
                        SentDate = itemEmails.Any()
                            ? itemEmails.Max(q => q.CreatedOn).ToRussianStandardTime()
                            : (DateTime?)null
                    };
                }).ToList()
            };

            return Json(vm);
        }
        
        [HttpPost]
        public async Task<IActionResult> SendPriceReductionRequests(
            [FromRoute] Guid id,
            [FromBody] SendPriceReductionRequestsVm model)
        {
            var userId = User.Id();
            var ownerId = User.CompanyId();
            
            BackgroundJob.Enqueue<PriceReductionJobs>(q => q.SendPriceReductionRequests(id, model, userId, ownerId));

            return Ok();
        }

        private PriceReductionData CreatePriceReductionData(SupplierOfferDetailsVm offer, CompetitionListVm cl,
            SendPriceReductionRequestsVm model = null)
        {
            var reportData = new PriceReductionData
            {
                InvoiceData = offer.InvoiceData,
                Currency = offer.Items.First(q => q.Offer.Qty > 0).Offer.Currency
            };

            foreach (var item in offer.Items.Where(q => q.Offer.Qty > 0))
            {
                SendPriceReductionRequestsVm.Item prData = null;

                if (model != null)
                {
                    prData = model.Items?.FirstOrDefault(q =>
                        q.SupplierOfferId == offer.Id
                        && q.ItemId == item.Request.ItemId);
                    if (prData == null) continue;
                }

                var haveTargetPrice = prData != null;
                decimal targetPrice;

                if (haveTargetPrice)
                {
                    targetPrice = prData.TargetPrice;
                }
                else
                {
                    var minimalPrice = cl.GetMinimalOfferPrice(item.Request.ItemId);
                    var defaultDiscount = 0.05m; // todo: get from database
                    var convertedTargetPrice = minimalPrice * (1 - defaultDiscount);
                    targetPrice = convertedTargetPrice;
                }
                
                var dataItem = new PriceReductionData.DataItem();
                dataItem
                    .SetPosition(item.Position)
                    .SetRequest(
                        item.Request.Code,
                        item.Request.Name,
                        item.Request.Uom,
                        item.Request.Qty)
                    .SetOffer(
                        item.Offer.Code,
                        item.Offer.Name,
                        item.Offer.Uom,
                        item.Offer.Qty,
                        item.Offer.Price)
                    .SetTargetPrice(item.Conversion.ToFinalCostCostPer1(targetPrice));

                reportData.Items.Add(dataItem);
            }

            return reportData;
        }
    }
}
