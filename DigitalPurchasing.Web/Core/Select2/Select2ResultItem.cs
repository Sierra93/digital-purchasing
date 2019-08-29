using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Core.Select2
{
    public class Select2ResultItem<T>
    {
        [JsonProperty("id")]
        public T Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
