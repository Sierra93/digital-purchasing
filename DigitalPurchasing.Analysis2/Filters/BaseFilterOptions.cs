using Newtonsoft.Json;

namespace DigitalPurchasing.Analysis2.Filters
{
    public abstract class BaseFilterOptions<TOptions> : IFilterOptions<TOptions> where TOptions : class, new()
    {
        public TOptions Options { get; set; } = new TOptions();

        public void DeserializeOptions(string str) => Options = JsonConvert.DeserializeObject<TOptions>(str);

        public string SerializeOptions() => JsonConvert.SerializeObject(Options);
    }
}
