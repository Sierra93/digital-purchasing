using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IQuotationRequestService
    {
        Task<int> CountByCompany(Guid companyId);

        QuotationRequestIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        Task<Guid> GetQuotationRequestId(Guid purchaseRequestId);
        QuotationRequestVm GetById(Guid id, bool globalSearch = false);
        QuotationRequestViewData GetViewData(Guid qrId);
        DeleteResultVm Delete(Guid id);
        Task SendRequests(Guid userId, Guid quotationRequestId, IReadOnlyList<Guid> suppliers,
            IReadOnlyList<(Guid SupplierId, Guid ItemId)> itemSuppliers);
        string QuotationRequestToUid(Guid quotationRequestId);
        Guid? UidToQuotationRequest(string uid);
        byte[] GenerateExcelByCategory(Guid quotationRequestId, params Guid[] categoryIds);
        byte[] GenerateExcelByItem(Guid quotationRequestId, params Guid[] itemIds);
        List<SentRequest> GetSentRequests(Guid quotationRequestId);
        string RequestSentBy(Guid quotationRequestId, string fromEmail);
    }

    public class QuotationRequestIndexDataItem
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PurchaseRequestCustomerName { get; set; }
        public string PurchaseRequestErpCode { get; set; }
    }

    public class QuotationRequestApplicableSupplier
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class QuotationRequestVm
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid OwnerId { get; set; }
        public Guid PurchaseRequestId { get; set; }
    }

    public class QuotationRequestIndexData : BaseDataResponse<QuotationRequestIndexDataItem>
    {

    }

    public class QuotationRequestViewData
    {
        public class NomenclatureItem
        {
            public string CompanyCode { get; set; }
            public string CompanyName { get; set; }
            public string CompanyUom { get; set; }
            public decimal CompanyQty { get; set; }

            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string CustomerUom { get; set; }
            public decimal CustomerQty { get; set; }

            public Guid CategoryId { get; set; }
            public Guid Id { get; set; }
        }

        public QuotationRequestViewData(string company, string customer)
        {
            SentRequests = new List<SentRequest>();
            ApplicableSuppliers = new List<QuotationRequestApplicableSupplier>();
            Items = new List<NomenclatureItem>();
            Company = company;
            Customer = customer;
        }

        public string Company { get; }
        public string Customer { get; }

        public List<QuotationRequestApplicableSupplier> ApplicableSuppliers { get; set; }

        public List<SentRequest> SentRequests { get; set; }

        public List<NomenclatureItem> Items { get; set; }
    }

    public class SentRequest
    {
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string PersonFullName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<Guid> CategoryIds { get; set; }
        public string PhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public bool ByCategory { get; set; }
        public bool ByItem { get; set; }
        public List<Guid> ItemIds { get; set; }
    }
}
