using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Analysis2.Filters
{
    public class VariantsItemQuantityFilter : VariantsFilter
    {
        public override List<List<AnalysisData>> Filter(List<List<AnalysisData>> allVariants, IAnalysisContext context)
        {


            //var variants = allVariants
            //    .Where(q => q
            //        .All(w => w.Item.Quantity >= GetCustomerQtyByItem(w.Item.Id, context)))
            //    .ToList();

            var v = allVariants
                .Where(
                    w => w
                        .GroupBy(g=> g.Item.Id)
                        .Select(q => new { ItemId = q.Key, Qty = q.Sum(e => e.Item.Quantity) })
                        .All(r => r.Qty >= GetCustomerQtyByItem(r.ItemId, context)))
                .ToList();

            return v;
        }

        private decimal GetCustomerQtyByItem(Guid itemId, IAnalysisContext context)
            => context.Customer.Items.Find(e => e.Id == itemId).Quantity;
    }
}
