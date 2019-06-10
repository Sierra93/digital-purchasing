using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Core.Interfaces.Analysis
{
    public readonly struct AnalysisResult
    {
        public Guid VariantId { get;  }

        public List<AnalysisResultData> Data { get; }

        public AnalysisResult(Guid variantId, IEnumerable<AnalysisResultData> data)
        {
            VariantId = variantId;
            Data = data.ToList();
        }

        public bool IsSuccess => Data.Any();

        public decimal TotalValue => Data?.Sum(q => q.TotalPrice) ?? 0;

        public Dictionary<Guid, decimal> GetTotalBySupplierOffer()
        {
            var result = new Dictionary<Guid, decimal>();

            if (TotalValue == 0) return result;

            foreach (var data in Data)
            {
                if (result.ContainsKey(data.SupplierOfferId))
                {
                    result[data.SupplierOfferId] += data.TotalPrice;
                }
                else
                {
                    result.Add(data.SupplierOfferId, data.TotalPrice);
                }
            }

            return result;
        }

        public int SuppliersCount => Data?.Select(q => q.SupplierId).Distinct().Count() ?? 0;

        public static AnalysisResult Empty(Guid variantId) => new AnalysisResult(variantId, new List<AnalysisResultData>());
    }
}
