using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ISupplierOfferService
    {
        void UpdateStatus(Guid id, SupplierOfferStatus status, bool globalSearch = false);
        void UpdateSupplierName(Guid id, string name, Guid? supplierId, bool globalSearch = false);
        void UpdateDeliveryCost(Guid id, decimal deliveryCost);

        Task<CreateFromFileResponse> CreateFromFile(Guid competitionListId, string filePath);

        SupplierOfferVm GetById(Guid id, bool globalSearch = false);
        SupplierOfferDetailsVm GetDetailsById(Guid id);

        SupplierOfferColumnsDataVm GetColumnsData(Guid id, bool globalSearch = false);
        void SaveColumns(Guid supplierOfferId, SupplierOfferColumnsVm columns, bool globalSearch = false);

        void GenerateRawItems(Guid id, bool globalSearch = false);

        SOMatchItemsVm MatchItemsData(Guid id);
        void SaveMatch(Guid soItemId, Guid nomenclatureId, Guid uomId, decimal factorC, decimal factorN);

        DeleteResultVm Delete(Guid id);

        SoTermsVm GetTerms(Guid supplierOfferId);
        void SaveTerms(SoTermsVm req, Guid supplierOfferId);
        bool IsAllMatched(Guid supplierOfferId);
        bool IsAllMatchedBySoItem(Guid soItemId);
        Task<Guid> GetCLIdBySoItem(Guid soItemId);
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

        public class ImportAndDeliveryData
        {
            private readonly Item _item;

            public ImportAndDeliveryData(Item item) => _item = item;

            public decimal CustomsDutyPerc => 0;
            public decimal MinCustomsDuty => 0;

            public DeliveryTerms DeliveryTerms { get; set; }

            public decimal CustomsDuty
            {
                get
                {
                    if (CustomsDutyPerc == 0) return 0;

                    if (DeliveryTerms == DeliveryTerms.CustomerWarehouse || DeliveryTerms == DeliveryTerms.DDP)
                        return 0;

                    var duty = _item.Offer.TotalPrice * CustomsDutyPerc;
                    if (duty < MinCustomsDuty)
                    {
                        duty = MinCustomsDuty;
                    }

                    return duty;
                }
            }

            public decimal TotalDeliveryCost { get; set; }

            public decimal DeliveryCost => TotalDeliveryCost * (_item.Mass.TotalMassPerc != 0 ? _item.Mass.TotalMassPerc :  _item.Mass.TotalPricePerc);

            public decimal FinalCost => CustomsDuty + DeliveryCost + _item.Offer.TotalPrice;
            
            public decimal FinalCostCostPer1 => _item.Offer.Qty != 0 ? FinalCost / _item.Offer.Qty : 0;
        }

        public class ConversionData
        {
            private readonly Item _item;

            public ConversionData(Item item) => _item = item;

            public decimal UomRatio { get; set; } = 1;

            public decimal CurrencyExchangeRate { get; set; } = 1;

            public decimal OfferQty => _item.Offer.Qty * UomRatio;

            public decimal OfferPrice => _item.ImportAndDelivery.FinalCostCostPer1 * CurrencyExchangeRate / UomRatio;

            public decimal OfferTotalPrice => OfferQty * OfferPrice;
        }

        public class ResourceConversionData
        {
            private readonly Item _item;

            public ResourceConversionData(Item item) => _item = item;

            public string ResourceUom { get; set; }
            public string ResourceBatchUom { get; set; }

            public decimal RequestResource { get; set; } = 1;
            public decimal OfferResource { get; set; } = 1;
            public decimal ResourceRatio => OfferResource / RequestResource;
            public decimal OfferPrice => _item.Conversion.OfferPrice * ResourceRatio;
            public decimal OfferTotalPrice => _item.Conversion.OfferTotalPrice * ResourceRatio;
        }

        public class Item
        {
            internal List<Item> Items { get; }

            public Item(List<Item> items)
            {
                Items = items;
                Request = new RequestData();
                Offer = new OfferData();
                Mass = new MassData(this);
                ResourceConversion = new ResourceConversionData(this);
                ImportAndDelivery = new ImportAndDeliveryData(this);
                Conversion = new ConversionData(this);
            }

            public RequestData Request { get; set; }
            public OfferData Offer { get; set; }
            public MassData Mass { get; set; }
            public ImportAndDeliveryData ImportAndDelivery { get; set; } 
            public ConversionData Conversion { get; set; }
            public ResourceConversionData ResourceConversion { get; set; }
        }

        #endregion

        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public DateTime CreatedOn { get; set; }

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
        public Guid? SupplierId { get; set; }
        public string CompanyName { get; set; }
        public decimal DeliveryCost { get; set; }
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
        public Guid? SupplierId { get; set; }
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
