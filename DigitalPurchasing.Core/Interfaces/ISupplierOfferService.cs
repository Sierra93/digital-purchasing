using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Extensions;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ISupplierOfferService
    {
        void UpdateStatus(Guid id, SupplierOfferStatus status);
        void UpdateSupplierName(Guid id, string name);

        CreateFromFileResponse CreateFromFile(Guid competitionListId, string filePath);

        SupplierOfferVm GetById(Guid id);
        SupplierOfferDetailsVm GetDetailsById(Guid id);

        SupplierOfferColumnsDataVm GetColumnsData(Guid id);
        void SaveColumns(Guid supplierOfferId, SupplierOfferColumnsVm columns);

        void GenerateRawItems(Guid id);

        SOMatchItemsVm MatchItemsData(Guid id);
        void SaveMatch(Guid itemId, Guid nomenclatureId, Guid uomId, decimal factorC, decimal factorN);

        DeleteResultVm Delete(Guid id);

        SoTermsVm GetTerms(Guid soId);
        void SaveTerms(SoTermsVm req, Guid soId);
    }

    public class SupplierOfferDetailsVm
    {
        #region Internal classes

        public class BaseData
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Currency { get; set; }
            public string Uom { get; set; }
            public decimal Qty { get; set; }
        }

        public class RequestData : BaseData
        {
        }

        public class OfferData : BaseData
        {
            public decimal Price { get; set; }
            public decimal TotalPrice => Qty * Price;
        }

        public class MassData
        {
            private readonly Item _item;

            public MassData(Item item) => _item = item;

            public decimal TotalPricePerc
            {
                get
                {
                    var sum = _item.Items.Sum(q => q.Offer.TotalPrice);
                    if (sum == 0) return 0;
                    return _item.Offer.TotalPrice / _item.Items.Sum(q => q.Offer.TotalPrice);
                }
            }

            public decimal MassOf1 { get; set; }
            public string MassUom { get; set; }

            public decimal TotalMass => _item.Offer.Qty * MassOf1;

            public decimal TotalMassPerc
            {
                get
                {
                    var sum = _item.Items.Sum(q => q.Mass.TotalMass);
                    if (sum == 0) return 0;
                    return TotalMass / sum;
                }
            }
        }

        public class Item
        {
            internal List<Item> Items { get; }

            public Item(List<Item> items) => Items = items;

            public RequestData Request { get; set; } = new RequestData();
            public OfferData Offer { get; set; } = new OfferData();
            public MassData Mass => new MassData(this);
        }

        #endregion

        public Guid Id { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();
    }

    public class SoTermsVm
    {
        public class Option
        {
            public string Text { get; set; }
            public int Value { get; set; }
            public int Order { get; set; }
        }

        public DateTime? ConfirmationDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public int PriceFixedForDays { get; set; }
        public int ReservedForDays { get; set; }
        public int DeliveryAfterConfirmationDays { get; set; }

        public DeliveryTerms DeliveryTerms { get; set; }
        public PaymentTerms PaymentTerms { get; set; }

        public int PayWithinDays { get; set; }

        public IEnumerable<Option> DeliveryTermsOptions { get; set; }

        public IEnumerable<Option> PaymentTermsOptions { get; set; }
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
