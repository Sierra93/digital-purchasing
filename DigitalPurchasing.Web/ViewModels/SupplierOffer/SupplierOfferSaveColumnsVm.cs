using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.ViewModels.SupplierOffer
{
    public class SupplierOfferSaveColumnsVm : SupplierOfferColumnsVm
    {
        [JsonProperty("soId")]
        public Guid SupplierOfferId { get; set; }
    }
}
