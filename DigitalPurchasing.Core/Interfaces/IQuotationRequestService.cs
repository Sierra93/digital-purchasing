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
        Task SendRequests(Guid userId, Guid quotationRequestId, IReadOnlyList<Guid> suppliers);
        string QuotationRequestToUid(Guid quotationRequestId);
        Guid? UidToQuotationRequest(string uid);
        byte[] GenerateExcelForQR(Guid quotationRequestId, params Guid[] categoryIds);
        List<SentRequest> GetSentRequests(Guid quotationRequestId);
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
        private readonly string _company;
        private readonly string _customer;

        public class NomenclatureItem
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Uom { get; set; }
            public string Qty { get; set; }
            public Guid CategoryId { get; set; }
        }

        public QuotationRequestViewData(string company, string customer)
        {
            SentRequests = new List<SentRequest>();
            ApplicableSuppliers = new List<QuotationRequestApplicableSupplier>();
            Items = new Dictionary<string, List<NomenclatureItem>>
            {
                { company, new List<NomenclatureItem>() },
                { customer, new List<NomenclatureItem>() }
            };
            _company = company;
            _customer = customer;
        }

        public List<QuotationRequestApplicableSupplier> ApplicableSuppliers { get; set; }

        public List<SentRequest> SentRequests { get; set; }

        public Dictionary<string, List<NomenclatureItem>> Items { get; set; }

        private void AddItem(string key, NomenclatureItem item) => Items[key].Add(item);

        public void AddCompanyItem(Guid categoryId, string name, string code, string uom, string qty)
            => AddItem(_company, new NomenclatureItem { Code = code, Name = name, Uom = uom, Qty = qty, CategoryId = categoryId });

        public void AddCustomerItem(Guid categoryId, string name, string code, string uom, string qty)
            => AddItem(_customer, new NomenclatureItem { Code = code, Name = name, Uom = uom, Qty = qty, CategoryId = categoryId });

        public IEnumerable<NomenclatureItem> GetCompanyItems() => Items[_company];
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
    }
}
