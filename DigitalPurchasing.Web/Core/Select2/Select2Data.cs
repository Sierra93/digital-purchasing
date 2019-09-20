using System.Collections.Generic;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Core.Select2
{
    public class Select2Data<T>
    {
        [JsonProperty("results")]
        public List<Select2ResultItem<T>> Results { get; set; }

        [JsonProperty("pagination")]
        public Select2Pagination Pagination { get; set; }

        public Select2Data() => Results = new List<Select2ResultItem<T>>();

        public Select2Data(bool more) : this() => Pagination = new Select2Pagination(more);
    }
}
