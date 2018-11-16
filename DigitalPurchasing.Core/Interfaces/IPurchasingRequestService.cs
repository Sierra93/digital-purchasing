using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IPurchaseRequestService
    {
        CreateFromFileResponse CreateFromFile(string filePath);
        PurchaseRequestDetailsResponse GetById(Guid id);
        PurchaseRequestColumnsResponse GetColumnsById(Guid id);
        void SaveColumns(Guid id, PurchaseRequestColumns purchasingRequestColumns);
        PurchaseRequestIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        void GenerateRawItems(Guid id);
        RawItemResponse GetRawItems(Guid id);
        void SaveRawItems(Guid id, IEnumerable<RawItemResponse.RawItem> items);
        void UpdateStatus(Guid id, PurchaseRequestStatus status);
        PRMatchItemsResponse MatchItemsData(Guid id);
        void SaveMatch(Guid id, Guid nomenclatureId, Guid uomId, decimal factorC, decimal factorN);
        void SaveCompanyName(Guid prId, string companyName);
        void SaveCustomerName(Guid prId, string customerName);
        DeleteResultVm Delete(Guid id);
    }

    public class CreateFromFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Guid Id { get; set; }
    }

    public class PurchaseRequestDetailsResponse
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }

        public string CompanyName { get; set; }
        public string CustomerName { get; set; }

        public DateTime CreatedOn { get; set; }

        public PurchaseRequestStatus Status { get; set; } 

        public ExcelTable ExcelTable { get; set; }

        public bool IsUploaded => ExcelTable != null;
    }

    public class PurchasingRequestIndexDataItem
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CustomerName { get; set; }
    }

    public class PurchaseRequestIndexData : BaseDataResponse<PurchasingRequestIndexDataItem>
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

    public class PurchaseRequestColumns
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }
        public string Qty { get; set; }
    }

    public class PurchaseRequestColumnsResponse : PurchaseRequestColumns
    {
        public List<string> Columns { get; set; }
        public bool IsSaved { get; set; }
    }

    public class PRMatchItemsResponse
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
