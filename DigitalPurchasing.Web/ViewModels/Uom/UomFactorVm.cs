using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.ViewModels.Uom
{
    public class UomFactorVm
    {
        [JsonProperty("from")]
        public Guid FromId { get; set; }
        [JsonProperty("to")]
        public Guid ToId { get; set; }
        [JsonProperty("nom")]
        public Guid NomenclatureId { get; set; }

        [JsonProperty("customerId")]
        public Guid? CustomerId { get; set; }

        [JsonProperty("supplierId")]
        public Guid? SupplierId { get; set; }
    }
}
