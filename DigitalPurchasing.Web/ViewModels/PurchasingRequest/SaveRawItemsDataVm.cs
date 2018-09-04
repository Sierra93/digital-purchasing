using System;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Web.ViewModels.PurchasingRequest
{
    public class SaveRawItemsDataVm : RawItemResponse
    {
        public Guid PurchasingRequestId { get; set; }
    }
}
