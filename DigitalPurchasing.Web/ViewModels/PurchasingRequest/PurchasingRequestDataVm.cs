using System;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Web.ViewModels
{
    public class PurchasingRequestDataVm : PurchasingRequestData
    {
        public string EditUrl { get; set; }
    }

    public class SavePurchasingRequestColumnsVm : PurchasingRequestColumns
    {
        public Guid PurchasingRequestId { get; set; }
    } 
}
