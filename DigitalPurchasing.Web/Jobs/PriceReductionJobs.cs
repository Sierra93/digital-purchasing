using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.ExcelReader;
using DigitalPurchasing.Models.Identity;
using DigitalPurchasing.Web.ViewModels.CompetitionList;
using Microsoft.AspNetCore.Identity;

namespace DigitalPurchasing.Web.Jobs
{
    public class PriceReductionJobs
    {
        private readonly IRootService _rootService;
        private readonly IQuotationRequestService _quotationRequestService;
        private readonly ICompetitionListService _competitionListService;
        private readonly ISupplierService _supplierService;
        private readonly ISupplierOfferService _supplierOfferService;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public PriceReductionJobs(
            ICompetitionListService competitionListService,
            UserManager<User> userManager,
            IUserService userService,
            ISupplierService supplierService,
            IRootService rootService,
            IQuotationRequestService quotationRequestService,
            ISupplierOfferService supplierOfferService)
        {
            _competitionListService = competitionListService;
            _userManager = userManager;
            _userService = userService;
            _supplierService = supplierService;
            _rootService = rootService;
            _quotationRequestService = quotationRequestService;
            _supplierOfferService = supplierOfferService;
        }

        public async Task SendPriceReductionRequests(
            Guid competitionListId,
            SendPriceReductionRequestsVm model, Guid userId, Guid companyId)
        {
            var cl = _competitionListService.GetById(competitionListId, true);
            if (cl == null) throw new NullReferenceException(nameof(cl));


            var userInfo = _userService.GetUserInfo(userId);
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var supplierOffersIds = model.Items.Select(q => q.SupplierOfferId).Distinct().ToList();

            var offersBySupplier = cl.GroupBySupplier();

            foreach (var supplierOfferId in supplierOffersIds)
            {
                var so = _supplierOfferService.GetById(supplierOfferId, true);

                if (!so.SupplierId.HasValue) continue;

                var supplierId = so.SupplierId.Value;

                var offers = offersBySupplier.First(q => q.Key.SupplierId == so.SupplierId.Value);
                var lastOffer = offers.Value.Last();
                var reportData = CreatePriceReductionData(lastOffer, cl, user, model);
                if (!reportData.Items.Any())
                {
                    continue;
                }

                SupplierContactPersonVm supplierContactPerson;

                if (so.ContactPersonId.HasValue)
                {
                    supplierContactPerson = _supplierService.GetContactPersonsById(so.ContactPersonId.Value);
                }
                else
                {
                    supplierContactPerson = _supplierService
                        .GetContactPersonsBySupplier(supplierId, true)
                        .FirstOrDefault();
                }

                if (supplierContactPerson == null) continue;

                var report = new PriceReductionWriter(reportData);
                var fileBytes = report.Build();
                var fileName = $"{cl.CreatedOn:yyyyMMdd}_КЛ_{cl.PublicId}_{lastOffer.SupplierName}_Запрос_на_изменение_условий.xlsx";
                var filePath = Path.Combine(Path.GetTempPath(), fileName);

                File.WriteAllBytes(filePath, fileBytes);

                var root = _rootService.GetByCL(cl.Id).Result;
                var emailUid = _quotationRequestService.QuotationRequestToUid(root.QuotationRequestId.Value);

                Hangfire.BackgroundJob.Enqueue<EmailJobs>(q
                    => q.SendPriceReductionEmail(companyId, emailUid, filePath, supplierContactPerson, userInfo,
                        DateTime.UtcNow.AddHours(user.PriceReductionResponseHours),
                        lastOffer.InvoiceData));

                var itemIds = model.Items
                    .Where(q => q.SupplierOfferId == supplierOfferId)
                    .Select(q => q.ItemId)
                    .ToList();

                await _competitionListService.SavePriceReductionEmail(
                    supplierOfferId,
                    supplierContactPerson.Id,
                    userId, itemIds);
            }
        }

        private PriceReductionData CreatePriceReductionData(SupplierOfferDetailsVm offer, CompetitionListVm cl, User user,
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
                    var defaultDiscount = (decimal)user.PRDiscountPercentage / 100m;
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
