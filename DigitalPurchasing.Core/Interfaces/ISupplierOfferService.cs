using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ISupplierOfferService
    {
        CreateFromFileResponse CreateFromFile(Guid competitionListId, string filePath);
        SupplierOfferVm GetById(Guid id);

        SupplierOfferColumnsDataVm GetColumnsData(Guid id);
        void SaveColumns(Guid supplierOfferId, SupplierOfferColumnsVm columns);
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
        public CompetitionListVm CompetitionList { get; set; }
        public UploadedDocumentVm UploadedDocument { get; set; }
        public SupplierOfferStatus Status { get; set; }
        public ExcelTable ExcelTable { get; set; }
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
        public List<string> Columns { get; set; }
    }
}
