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
        MatchItemsResponse MatchItemsData(Guid id);
        void SaveMatch(Guid id, Guid nomenclatureId, Guid uomId, decimal factorC, decimal factorN);
        void SaveCompanyName(Guid prId, string companyName);
        void SaveCustomerName(Guid prId, string customerName);
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

        public string CompanyName { get; set; }
        public string CustomerName { get; set; }

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
            public string RawCode { get; set; }
            public string RawName { get; set; }
            public string RawUom { get; set; }
            public decimal RawQty { get; set; }
        }

        public List<RawItem> Items { get; set; }

        public string CustomerName { get; set; }
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

    public class MatchItemsResponse
    {
        public class Item
        {
            public Guid Id { get; set; }

            public string RawCode { get; set; }
            public string RawName { get; set; }
            public string RawUom { get; set; }
            public decimal RawQty { get; set; }
            
            public Guid? RawUomMatchId { get; set; }

            public Guid? NomenclatureId { get; set; }
            public string NomenclatureName { get; set; }
            public string NomenclatureCode { get; set; }
            public string NomenclatureUom { get; set; }

            public decimal NomenclatureFactor { get; set; }
            public decimal CommonFactor { get; set; }
        }

        public List<Item> Items { get; set; } = new List<Item>();

        public string CustomerName { get; set; }
    }
}
