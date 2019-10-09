using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Enums;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICompetitionListService
    {
        Task<int> CountByCompany(Guid companyId);

        CompetitionListIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        Task<Guid> GetIdByQR(Guid qrId, bool globalSearch);
        CompetitionListVm GetById(Guid id, bool globalSearch = false);
        DeleteResultVm Delete(Guid id);

        Task SavePriceReductionEmail(
            Guid supplierOfferId,
            Guid supplierContactPersonId,
            Guid? userId,
            List<Guid> ids,
            PriceReductionSendingType sender);

        Task<bool> IsPriceReductionEmailSent(Guid supplierOfferId);
        Task<IEnumerable<PriceReductionEmailDto>> GetPriceReductionEmailsByCL(Guid competitionListId);

        Task Close(Guid competitionListId);
        Task CloseExpired();
        Task<bool> IsAutomaticCloseDateSet(Guid competitionListId);
        Task SetAutomaticCloseInHours(Guid competitionListId, double hours);
        Task<PriceReductionDataVm> PriceReductionData(Guid competitionListId, Guid userId);
    }

    public class CompetitionListIndexDataItem
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string QuotationRequestPurchaseRequestCustomerName { get; set; }
        public string QuotationRequestPurchaseRequestErpCode { get; set; }
        public string Suppliers { get; set; }
        public bool? IsClosed { get; set; }
        public DateTime AutomaticCloseDate { get; set; }
    }

    public class CompetitionListIndexData : BaseDataResponse<CompetitionListIndexDataItem>
    {
    }

    public class CompetitionListVm
    {
        public readonly struct SOGroup
        {
            public DateTime CreatedOn { get; }
            public Guid? SupplierId { get; }

            public SOGroup(DateTime createdOn, Guid? supplierId)
            {
                CreatedOn = createdOn;
                SupplierId = supplierId;
            }
        }

        public class CustomerVm
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class PurchaseRequestVm
        {
            public CustomerVm Customer { get; set; }

            public Guid Id { get; set; }
            public int PublicId { get; set; }
            public DateTime CreatedOn { get; set; }
            public string CustomerName { get; set; }
            public string ErpCode { get; set; }
            public List<PurchaseRequestItemVm> Items { get; set; }
        }

        public class PurchaseRequestItemVm
        {
            public Guid Id { get; set; }
            public Guid NomenclatureId { get; set; }
            public NomenclatureVm Nomenclature { get; set; }
            public int Position { get; set; }
            public string RawCode { get; set; }
            public string RawName { get; set; }
            public string RawUom { get; set; }
            public decimal RawQty { get; set; }
        }

        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }

        public PurchaseRequestVm PurchaseRequest { get; set; }
        public List<SupplierOfferDetailsVm> SupplierOffers { get; set; }

        public Dictionary<SOGroup, List<SupplierOfferDetailsVm>> GroupBySupplier()
        {
            var result = new Dictionary<SOGroup, List<SupplierOfferDetailsVm>>();

            foreach (var grouping in SupplierOffers.GroupBy(q => q.SupplierId))
            {
                if (grouping.Key.HasValue)
                {
                    var minDate = grouping.Min(q => q.CreatedOn);
                    var supplierId = grouping.First().SupplierId;
                    result.Add(new SOGroup(minDate, supplierId), grouping.OrderBy(q => q.CreatedOn).ToList());
                }
                else
                {
                    foreach (var offer in grouping)
                    {
                        result.Add(new SOGroup(offer.CreatedOn, offer.SupplierId), new List<SupplierOfferDetailsVm> { offer });
                    }
                }
            }

            return result;
        }

        public decimal GetMinimalOfferPrice(Guid prItemId)
        {
            var soItems = SupplierOffers
                .SelectMany(so
                    => so.Items.Where(soi
                        => soi.Request.ItemId == prItemId && soi.Offer.Price > 0))
                .ToList();

            return soItems.Any()
                ? soItems.Min(soi => soi.ResourceConversion.OfferPrice)
                : -1;
        }

        public List<Guid> GetSOIdsWMinimalPrice(Guid prItemId)
        {
            var soItems = SupplierOffers
                .SelectMany(so
                    => so.Items.Where(soi
                        => soi.Request.ItemId == prItemId && soi.Offer.Price > 0))
                .ToList();

            var minPrice = soItems.Any()
                ? soItems.Min(soi => soi.ResourceConversion.OfferPrice)
                : -1;

            if (minPrice == -1) return new List<Guid>();

            var soIds = soItems
                .Where(soi => soi.ResourceConversion.OfferPrice == minPrice)
                .Select(q => q.Offer.ItemId);

            return SupplierOffers
                .Where(q => q.Items.Any(i => soIds.Contains(i.Offer.ItemId)))
                .Select(q => q.Id)
                .ToList();
        }
    }

    public static class Extensions
    {
        public static Dictionary<CompetitionListVm.SOGroup, SupplierOfferDetailsVm> Merge(
            this Dictionary<CompetitionListVm.SOGroup, List<SupplierOfferDetailsVm>> value)
        {
            var result = value.ToDictionary(
                group => group.Key,
                group => group.Value.Merge());

            return result;
        }

        public static SupplierOfferDetailsVm Merge(this List<SupplierOfferDetailsVm> offers)
        {
            var lastOffer = offers.Last();
            var lastOfferItemsCount = lastOffer.Items.Count;
            lastOffer.Items = lastOffer.Items.Where(q => q.Offer.Qty > 0).ToList();
            if (lastOffer.Items.Count < lastOfferItemsCount)
            {
                foreach (var offer in offers.Where(q => q.Id != lastOffer.Id))
                {
                    var items = offer.Items.Where(item =>
                        item.Offer.Qty > 0 && lastOffer.Items.All(q => q.NomenclatureId != item.NomenclatureId));
                    lastOffer.Items.AddRange(items);
                }
            }
            return lastOffer;
        }
    }

    public class PriceReductionEmailDto
    {
        public Guid SupplierOfferId { get; set; }
        public Guid ContactPersonId { get; set; }
        public Guid? UserId { get; set; }
        public List<Guid> Data { get; set; }
        public DateTime CreatedOn { get; set; }
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
            public List<Guid> MinPriceSOIds { get; set; }
        }

        public List<Supplier> Suppliers { get; set; }
        public List<Item> Items { get; set; }
    }

}
