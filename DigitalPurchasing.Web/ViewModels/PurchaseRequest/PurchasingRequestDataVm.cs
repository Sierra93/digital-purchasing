using System;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Web.ViewModels
{
    public class PurchaseRequestDataVm : PurchasingRequestData
    {
        public string EditUrl { get; set; }
    }

    public class SavePurchaseRequestColumnsVm : PurchaseRequestColumns
    {
        public Guid PurchaseRequestId { get; set; }
    }

    public class SaveMatchItemVm
    {
        public Guid ItemId { get; set; }
        public Guid NomenclatureId { get; set; }
        public Guid UomId { get; set; }
        public decimal FactorN { get; set; }
        public decimal FactorC { get; set; }
    }
}
