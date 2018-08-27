using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IPurchasingRequestService
    {
        Guid CreateFromFile(string filePath);
        PurchasingRequestDetailsResponse GetById(Guid id);
        PurchasingRequestDataResponse GetData(int page, int perPage, string sortField, bool sortAsc);
    }

    public enum PurchasingRequestType
    {
        AppropriateColumns = 0,
        ManualInput = 10
    }

    public class PurchasingRequestDetailsResponse
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }

        public DateTime CreatedOn { get; set; }

        public PurchasingRequestType Type { get; set; } 

        public ExcelTable ExcelTable { get; set; }

        public bool IsUploaded => ExcelTable != null;
    }

    public class PurchasingRequestData
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class PurchasingRequestDataResponse : BaseDataResponse<PurchasingRequestData>
    {
    }
}
