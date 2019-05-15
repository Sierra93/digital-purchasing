using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Analysis2
{
    public class AnalysisResult
    {
        public Guid VariantId { get;  }

        public List<AnalysisData> Data { get; }

        public AnalysisResult(Guid variantId, List<AnalysisData> data)
        {
            VariantId = variantId;
            Data = data;
        }

        public bool IsSuccess => Data.Any();

        public decimal TotalValue => Data?.Sum(q => q.Item.TotalPrice) ?? 0;

        public Dictionary<Guid, decimal> GetTotalBySupplier()
        {
            var result = new Dictionary<Guid, decimal>();

            if (TotalValue == 0) return result;

            foreach (var data in Data)
            {
                if (result.ContainsKey(data.SupplierId))
                {
                    result[data.SupplierId] += data.Item.TotalPrice;
                }
                else
                {
                    result.Add(data.SupplierId, data.Item.TotalPrice);
                }
            }

            return result;
        }

        public int SuppliersCount => Data?.Select(q => q.SupplierId).Distinct().Count() ?? 0;

        public static AnalysisResult Empty(Guid variantId) => new AnalysisResult(variantId, new List<AnalysisData>());
    }
}
