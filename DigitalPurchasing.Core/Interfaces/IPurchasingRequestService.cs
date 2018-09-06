using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IPurchasingRequestService
    {
        CreateFromFileResponse CreateFromFile(string filePath);
        PurchasingRequestDetailsResponse GetById(Guid id);
        PurchasingRequestColumnsResponse GetColumnsById(Guid id);
        void SaveColumns(Guid id, PurchasingRequestColumns purchasingRequestColumns);
        PurchasingRequestDataResponse GetData(int page, int perPage, string sortField, bool sortAsc);
        void GenerateRawItems(Guid id);
        RawItemResponse GetRawItems(Guid id);
        void SaveRawItems(Guid id, IEnumerable<RawItemResponse.RawItem> items);
        void UpdateStatus(Guid id, PurchasingRequestStatus status);
    }

    public enum PurchasingRequestStatus
    {
        MatchColumns = -10,
        ManualInput = 0,
        MatchItems = 10
    }

    public class CreateFromFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Guid Id { get; set; }
    }

    public class PurchasingRequestDetailsResponse
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }

        public DateTime CreatedOn { get; set; }

        public PurchasingRequestStatus Status { get; set; } 

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

    public class RawItemResponse
    {
        public class RawItem
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Uom { get; set; }
            public decimal Qty { get; set; }
        }

        public List<RawItem> Items { get; set; }
    }

    public class PurchasingRequestColumns
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }
        public string Qty { get; set; }
    }

    public class PurchasingRequestColumnsResponse : PurchasingRequestColumns
    {
        public List<string> Columns { get; set; }
        public bool IsSaved { get; set; }
    }
}
