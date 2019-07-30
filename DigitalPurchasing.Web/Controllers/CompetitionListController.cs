using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.ExcelReader;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.CompetitionList;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class CompetitionListController : BaseController
    {
        private readonly ICompetitionListService _competitionListService;
        private readonly ISupplierOfferService _supplierOfferService;
        private readonly ISelectedSupplierService _selectedSupplierService;

        public CompetitionListController(
            ICompetitionListService competitionListService,
            ISupplierOfferService supplierOfferService,
            ISelectedSupplierService selectedSupplierService)
        {
            _competitionListService = competitionListService;
            _supplierOfferService = supplierOfferService;
            _selectedSupplierService = selectedSupplierService;
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
            var cl = _competitionListService.GetById(id);
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
            }

            public class Item
            {
                public int Position { get; set; }
                public decimal MinPrice { get; set; }
                public decimal TargetPrice => MinPrice > 0 ? Math.Round(MinPrice * ( 1 - Discount/100), 2) : -1;
                public decimal Discount { get; set; }
                public List<ItemSupplier> Suppliers { get; set; }
                public Guid Id { get; set; }
            }

            public List<Supplier> Suppliers { get; set; }
            public List<Item> Items { get; set; }
        }

        [HttpGet]
        public IActionResult PriceReductionData([FromRoute]Guid id)
        {
            var cl = _competitionListService.GetById(id);

            var vm = new PriceReductionDataVm
            {
                Suppliers = cl.SupplierOffers.OrderBy(q => q.CreatedOn).Select(q => new PriceReductionDataVm.Supplier
                {
                    Id = q.Id,
                    Name = q.SupplierName,
                    CreatedOn = q.CreatedOn,
                    IsChecked = true
                }).ToList(),

                Items = cl.PurchaseRequest.Items.Select(pri =>
                {
                    var soItems = cl.SupplierOffers
                        .SelectMany(so
                            => so.Items.Where(soi
                                => soi.Request.ItemId == pri.Id && soi.Offer.Price > 0))
                        .ToList();

                    return new PriceReductionDataVm.Item
                    {
                        Id = pri.Id,
                        Position = pri.Position,
                        Discount = 5m,
                        MinPrice = soItems.Any()
                            ? soItems.Min(soi => soi.ResourceConversion.OfferPrice)
                            : -1,
                        Suppliers = cl.SupplierOffers.OrderBy(q => q.CreatedOn).Select(so => new PriceReductionDataVm.ItemSupplier
                        {
                            Id = so.Id,
                            IsChecked = true,
                            IsEnabled = so.Items.Any(soi
                                => soi.Request.ItemId == pri.Id && soi.Offer.Price > 0)
                        }).ToList(),
                    };
                }).ToList()
            };

            return Json(vm);
        }
    }
}
