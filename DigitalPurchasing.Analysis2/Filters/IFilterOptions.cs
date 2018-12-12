namespace DigitalPurchasing.Analysis2.Filters
{
    public interface IFilterOptions<TOptions>
    {
        TOptions Options { get; set; }
        string SerializeOptions();
        void DeserializeOptions(string str);
    }
}