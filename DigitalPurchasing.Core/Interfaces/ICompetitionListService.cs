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
        CompetitionListVm GetById(Guid id);
        DeleteResultVm Delete(Guid id);

        Task SavePriceReductionEmail(
            Guid competitionListId,
            Guid supplierContactPersonId,
            Guid? userId,
            List<Guid> ids);

        Task<IEnumerable<PriceReductionEmailDto>> GetPriceReductionEmailsByCL(Guid competitionListId);
    }

    public class CompetitionListIndexDataItem
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string QuotationRequestPurchaseRequestCustomerName { get; set; }
        public string QuotationRequestPurchaseRequestErpCode { get; set; }
        public string Suppliers { get; set; }
    }

    public class CompetitionListIndexData : BaseDataResponse<CompetitionListIndexDataItem>
    {
    }

    public class CompetitionListVm
    {
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

        public Dictionary<(DateTime CreatedOn, Guid? SupplierId), List<SupplierOfferDetailsVm>> GroupBySupplier()
        {
            var result = new Dictionary<(DateTime CreatedOn, Guid? SupplierId), List<SupplierOfferDetailsVm>>();

            foreach (var grouping in SupplierOffers.GroupBy(q => q.SupplierId))
            {
                if (grouping.Key.HasValue)
                {
                    var minDate = grouping.Min(q => q.CreatedOn);
                    var supplierId = grouping.First().SupplierId;
                    result.Add(
                        (CreatedOn: minDate, SupplierId: supplierId),
                        grouping.OrderBy(q => q.CreatedOn).ToList());
                }
                else
                {
                    foreach (var offer in grouping)
                    {
                        result.Add(
                            (offer.CreatedOn, offer.SupplierId),
                            new List<SupplierOfferDetailsVm> { offer });
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
    }

    public class PriceReductionEmailDto
    {
        public Guid CompetitionListId { get; set; }

        public Guid ContactPersonId { get; set; }

        public Guid? UserId { get; set; }

        public List<Guid> Data { get; set; }
    }
}
