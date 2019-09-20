using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Core.Select2
{
    public class Select2Pagination
    {

        [JsonProperty("more")]
        public bool More { get; set; }

        public Select2Pagination()
        {
        }

        public Select2Pagination(bool more) => More = more;
    }
}
