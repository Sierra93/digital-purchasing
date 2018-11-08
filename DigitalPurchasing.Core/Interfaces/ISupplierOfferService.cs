using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ISupplierOfferService
    {
        void UpdateStatus(Guid id, SupplierOfferStatus status);
        void UpdateSupplierName(Guid id, string name);

        CreateFromFileResponse CreateFromFile(Guid competitionListId, string filePath);
        SupplierOfferVm GetById(Guid id);

        SupplierOfferColumnsDataVm GetColumnsData(Guid id);
        void SaveColumns(Guid supplierOfferId, SupplierOfferColumnsVm columns);

        void GenerateRawItems(Guid id);

        SOMatchItemsVm MatchItemsData(Guid id);
        void SaveMatch(Guid itemId, Guid nomenclatureId, Guid uomId, decimal factorC, decimal factorN);

    }

    public class UploadedDocumentVm
    {
        public string Data { get; set; }
        public UploadedDocumentHeadersVm Headers { get; set; }
    }

    public class UploadedDocumentHeadersVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }
        public string Qty { get; set; }
        public string Price { get; set; }
    }

    public class SupplierOfferVm
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }

        public CompetitionListVm CompetitionList { get; set; }
        public UploadedDocumentVm UploadedDocument { get; set; }
        public SupplierOfferStatus Status { get; set; }
        public ExcelTable ExcelTable { get; set; }

        public string SupplierName { get; set; }
        public string CompanyName { get; set; }
    }

    public class SupplierOfferColumnsVm
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }
        public string Qty { get; set; }
        public string Price { get; set; }
    }

    public class SupplierOfferColumnsDataVm : SupplierOfferColumnsVm
    {
        public string SupplierName { get; set; }
        public List<string> Columns { get; set; }
    }

    public class SOMatchItemsVm
    {
        public string SupplierName { get; set; }

        public class Item
        {
            public Guid Id { get; set; }
            public string RawCode { get; set; }
            public string RawName { get; set; }
            public string RawUomStr { get; set; }
            public decimal RawQty { get; set; }
            public decimal RawPrice { get; set; }

            public Guid? RawUomId { get; set; }

            public Guid? NomenclatureId { get; set; }
            public string NomenclatureName { get; set; }
            public string NomenclatureCode { get; set; }
            public string NomenclatureUom { get; set; }

            public decimal NomenclatureFactor { get; set; }
            public decimal CommonFactor { get; set; }
        }

        public List<Item> Items = new List<Item>();
    }
}
