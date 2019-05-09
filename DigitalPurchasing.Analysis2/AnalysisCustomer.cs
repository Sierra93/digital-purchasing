using System;

namespace DigitalPurchasing.Analysis2
{
    public class AnalysisCustomer : AnalysisClient<CustomerItem>, ICopyable<AnalysisCustomer>
    {
        public Guid CustomerId { get; set; }

        public AnalysisCustomer Copy() => new AnalysisCustomer
        {
            Id = Id, Date = Date, Items = Items.ConvertAll(q => q.Copy()), CustomerId = CustomerId
        };
    }
}
