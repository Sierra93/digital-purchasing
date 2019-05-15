using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Analysis2.Filters
{
    public class VariantsItemQuantityFilter : VariantsFilter
    {
        public override int Order => 30;

        public override List<List<AnalysisData>> Filter(List<List<AnalysisData>> variants, IAnalysisContext context)
        {
            var customerItems = context.Customer.Items.ToDictionary(q => q.Id);

            var fullVariants = variants.Where(q => q.All(w => w.Item.Quantity >= w.CustomerQuantity)).ToList();

            if (fullVariants.Count == variants.Count)
            {
                return fullVariants;
            }

            var partialVariants = variants.Except(fullVariants);
            
            var validPartialVariants = partialVariants
                .Where(
                    w => w
                        .GroupBy(g=> g.Item.Id)
                        .Select(q => new { ItemId = q.Key, Qty = q.Sum(e => e.Item.Quantity) })
                        .All(r => r.Qty >= customerItems[r.ItemId].Quantity))
                .ToList();
            
            return fullVariants.Union(validPartialVariants).ToList();
        }
    }
}
