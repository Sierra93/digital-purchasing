using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICompetitionListService
    {
        Task<int> CountByCompany(Guid companyId);

        CompetitionListIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        Guid GetIdByQR(Guid qrId, bool ignoreQueryFilters);
        CompetitionListVm GetById(Guid id);
        DeleteResultVm Delete(Guid id);
    }

    public class CompetitionListIndexDataItem
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class CompetitionListIndexData : BaseDataResponse<CompetitionListIndexDataItem>
    {
    }

    public class CompetitionListVm
    {
        public class PurchaseRequestVm
        {
            public Guid Id { get; set; }
            public int PublicId { get; set; }
            public DateTime CreatedOn { get; set; }
            public string CustomerName { get; set; }
            public List<PurchaseRequestItemVm> Items { get; set; }
        }

        public class PurchaseRequestItemVm
        {
            public Guid Id { get; set; }
            public Guid NomenclatureId { get; set; }
            public int Position { get; set; }
            public string RawCode { get; set; }
            public string RawName { get; set; }
            public string RawUom { get; set; }
            public decimal RawQty { get; set; }
        }

        public class SupplierOfferVm
        {
            public Guid Id { get; set; }
            public int PublicId { get; set; }
            public DateTime CreatedOn { get; set; }
            public string SupplierName { get; set; }
            public List<SupplierOfferItemVm> Items { get; set; }
            public CurrencyVm Currency { get; set; }
            public DeliveryTerms DeliveryTerms { get; set; }
            public PaymentTerms PaymentTerms { get; set; }
        }

        public class SupplierOfferItemVm
        {
            public Guid Id { get; set; }
            public Guid NomenclatureId { get; set; }
            public string RawCode { get; set; }
            public string RawName { get; set; }
            public string RawUom { get; set; }
            public decimal RawQty { get; set; }
            public decimal RawPrice { get; set; }
            public decimal TotalPrice => RawQty * RawPrice;
        }

        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }

        public PurchaseRequestVm PurchaseRequest { get; set; }
        public IEnumerable<SupplierOfferVm> SupplierOffers { get; set; }
    }
}
