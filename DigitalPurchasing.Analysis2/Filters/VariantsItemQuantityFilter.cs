using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Analysis2.Filters
{
    public class VariantsItemQuantityFilter : VariantsFilter
    {
        public override List<List<AnalysisData>> Filter(List<List<AnalysisData>> allVariants, IAnalysisContext context)
        {
            var customerItems = context.Customer.Items.ToDictionary(q => q.Id);
            var v = allVariants
                .Where(
                    w => w
                        .GroupBy(g=> g.Item.Id)
                        .Select(q => new { ItemId = q.Key, Qty = q.Sum(e => e.Item.Quantity) })
                        .All(r => r.Qty >= customerItems[r.ItemId].Quantity))
                .ToList();

            return v;
        }
    }
}
