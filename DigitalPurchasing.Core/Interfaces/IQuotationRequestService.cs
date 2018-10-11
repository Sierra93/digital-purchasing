using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IQuotationRequestService
    {
        QuotationRequestIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        Guid GetQuotationRequestId(Guid purchaseRequestId);
        QuotationRequestDetails GetById(Guid id);
    }

    public class QuotationRequestIndexDataItem
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class QuotationRequestDetails
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class QuotationRequestIndexData : BaseDataResponse<QuotationRequestIndexDataItem>
    {

    }
}
