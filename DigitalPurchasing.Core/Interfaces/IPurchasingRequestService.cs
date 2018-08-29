using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IPurchasingRequestService
    {
        Guid CreateFromFile(string filePath);
        PurchasingRequestDetailsResponse GetById(Guid id);
        PurchasingRequestColumnsResponse GetColumnsById(Guid id);
        void SaveColumns(Guid id, PurchasingRequestColumns purchasingRequestColumns);
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

    public class PurchasingRequestColumns
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }
        public string Qty { get; set; }
        public string Date { get; set; }
        public string Receiver { get; set; }
    }

    public class PurchasingRequestColumnsResponse : PurchasingRequestColumns
    {
        public List<string> Columns { get; set; }
        public bool IsSaved { get; set; }
    }
}
