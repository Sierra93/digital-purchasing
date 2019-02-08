namespace DigitalPurchasing.Analysis2
{
    public class AnalysisCustomer : AnalysisClient<CustomerItem>, ICopyable<AnalysisCustomer>
    {
        public AnalysisCustomer Copy() => new AnalysisCustomer
        {
            Id = Id, Date = Date, Items = Items.ConvertAll(q => q.Copy())
        };
    }
}
