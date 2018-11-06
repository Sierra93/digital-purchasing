using System;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Web.ViewModels
{
    public class PurchaseRequestDataVm : PurchasingRequestIndexDataItem
    {
        public string EditUrl { get; set; }
    }

    public class SavePurchaseRequestColumnsVm : PurchaseRequestColumns
    {
        public Guid PurchaseRequestId { get; set; }
    }
}
