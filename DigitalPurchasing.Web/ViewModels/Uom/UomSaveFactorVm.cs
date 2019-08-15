using System;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.ViewModels.Uom
{
    public class UomDeleteFactorVm
    {
        public Guid Id { get; set; }
    }

    public class UomSaveFactorVm
    {
        [JsonProperty("fromId")]
        public Guid FromUomId { get; set; }
        [JsonProperty("toId")]
        public Guid ToUomId { get; set; }
        [JsonProperty("nomenclatureAlternativeId")]
        public Guid? NomenclatureAlternativeId { get; set; }
        [JsonProperty("factorC")]
        public decimal FactorC { get; set; }
        [JsonProperty("factorN")]
        public decimal FactorN { get; set; }
    }
}
