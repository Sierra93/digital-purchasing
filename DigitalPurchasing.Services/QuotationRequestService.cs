using System;
using System.Collections.Generic;
using System.Globalization;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class QuotationRequestService : IQuotationRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICounterService _counterService;
        private readonly IPurchaseRequestService _purchaseRequestService;
        private readonly IDeliveryService _deliveryService;

        public QuotationRequestService(
            ApplicationDbContext db,
            ICounterService counterService,
            IPurchaseRequestService purchaseRequestService,
            IDeliveryService deliveryService)
        {
            _db = db;
            _counterService = counterService;
            _purchaseRequestService = purchaseRequestService;
            _deliveryService = deliveryService;
        }

        public QuotationRequestIndexData GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Id";
            }

            var qry =  _db.QuotationRequests.AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults
                .Skip((page-1)*perPage)
                .Take(perPage)
                .ProjectToType<QuotationRequestIndexDataItem>()
                .ToList();

            return new QuotationRequestIndexData
            {
                Data = result,
                Total = total
            };
        }

        private Guid CreateQuotationRequest(Guid purchaseRequestId)
        {
            var publicId = _counterService.GetQRNextId();
            var entry = _db.QuotationRequests.Add(new QuotationRequest {PublicId = publicId, PurchaseRequestId = purchaseRequestId});
            _db.SaveChanges();

            // copy delivery to QR
            var prDelivery = _deliveryService.GetByPrId(purchaseRequestId);
            prDelivery.Id = Guid.Empty;
            _deliveryService.CreateOrUpdate(prDelivery, null, entry.Entity.Id);
            
            return entry.Entity.Id;
        }

        public Guid GetQuotationRequestId(Guid purchaseRequestId)
        {
            var quotationRequest = _db.QuotationRequests.FirstOrDefault(q => q.PurchaseRequestId == purchaseRequestId);
            if (quotationRequest != null)
            {
                return quotationRequest.Id;
            }

            var data = _purchaseRequestService.MatchItemsData(purchaseRequestId);
            if (data == null) return Guid.Empty;

            if (data.Items.All(q => q.NomenclatureId.HasValue && q.RawUomMatchId.HasValue && (q.CommonFactor > 0 || q.NomenclatureFactor > 0 )))
            {
                return CreateQuotationRequest(purchaseRequestId);
            }

            return Guid.Empty;;
        }

        public QuotationRequestDetails GetById(Guid id)
        {
            var quotationRequest = _db.QuotationRequests.Find(id);
            if (quotationRequest == null) return null;

            var result = quotationRequest.Adapt<QuotationRequestDetails>();
            
            return result;
        }

        public QuotationRequestViewData GetViewData(Guid qrId)
        {
            var qr = _db.QuotationRequests.Find(qrId);
            var prId = qr.PurchaseRequestId;
            var pr = _db.PurchaseRequests.Find(prId);

            var data = _purchaseRequestService.MatchItemsData(prId);
            var companyName = pr.CompanyName;
            var customerName = pr.CustomerName;
            var result = new QuotationRequestViewData(companyName, customerName);

            foreach (var dataItem in data.Items)
            {
                var factor = dataItem.CommonFactor > 0 ? dataItem.CommonFactor : dataItem.NomenclatureFactor;
                var companyQty = dataItem.RawQty * factor;
                result.AddCompanyItem(dataItem.NomenclatureName, dataItem.NomenclatureCode, dataItem.NomenclatureUom, companyQty.ToString(CultureInfo.InvariantCulture));
                result.AddCustomerItem(dataItem.RawName, dataItem.RawCode, dataItem.RawUom, dataItem.RawQty.ToString(CultureInfo.InvariantCulture));
            }

            return result;
        }
    }
}
