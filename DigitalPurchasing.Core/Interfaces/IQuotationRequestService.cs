using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IQuotationRequestService
    {
        QuotationRequestIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        Guid GetQuotationRequestId(Guid purchaseRequestId);
        QuotationRequestVm GetById(Guid id);
        QuotationRequestViewData GetViewData(Guid qrId);
    }

    public class QuotationRequestIndexDataItem
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class QuotationRequestVm
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
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
        }

        public QuotationRequestViewData(string company, string customer)
        {
            Items = new Dictionary<string, List<NomenclatureItem>>
            {
                { company, new List<NomenclatureItem>() },
                { customer, new List<NomenclatureItem>() }
            };
            _company = company;
            _customer = customer;
        }

        public Dictionary<string, List<NomenclatureItem>> Items { get; set; }

        private void AddItem(string key, NomenclatureItem item) => Items[key].Add(item);

        public void AddCompanyItem(string name, string code, string uom, string qty)
            => AddItem(_company, new NomenclatureItem { Code = code, Name = name, Uom = uom, Qty = qty });

        public void AddCustomerItem(string name, string code, string uom, string qty)
            => AddItem(_customer, new NomenclatureItem { Code = code, Name = name, Uom = uom, Qty = qty });

        public IEnumerable<NomenclatureItem> GetCompanyItems() => Items[_company];
    }
}
