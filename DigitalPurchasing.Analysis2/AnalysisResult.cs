using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Analysis2
{
    public class AnalysisResult
    {
        public List<AnalysisData> Data { get; set; }

        public AnalysisResult(List<AnalysisData> data) => Data = data;

        public bool IsSuccess => Data.Any();

        public decimal TotalValue => Data?.Sum(q => q.Item.TotalPrice) ?? 0;
    }
}
