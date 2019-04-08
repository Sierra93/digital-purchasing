using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Analysis2.Filters;

namespace DigitalPurchasing.Analysis2
{
    public class AnalysisCore
    {
        public AnalysisCustomer Customer { get; set; }
        public List<AnalysisSupplier> Suppliers { get; set; } = new List<AnalysisSupplier>();

        public AnalysisResult Run(AnalysisCoreVariant options)
        {
            var suppliers = Suppliers;
            var customerItemIds = Customer.Items.Select(q => q.Id);

            var suppliersFilters = new List<ISuppliersFilter>
            {
                new SupplierDeliveryTermsFilter(options.DeliveryTermsOptions),
                new SupplierPaymentTermsFilter(options.PaymentTermsOptions),
                new SupplierDeliveryDateTermsFilter(options.DeliveryDateTermsOptions)
            };

            var context = new AnalysisContext { Customer = Customer };

            foreach (var filter in suppliersFilters)
            {
                suppliers = filter.Filter(suppliers, context);
            }

            var datas = new List<AnalysisData>();

            foreach (var supplier in suppliers)
            {
                var items = supplier.Items.Where(q => customerItemIds.Contains(q.Id));
                foreach (var item in items)
                {
                    datas.Add(new AnalysisData { Supplier = supplier, Item = item });
                }
            }

            var itemPairs = GenerateItemPairs(datas);

            var coreVariants = itemPairs.CartesianProduct();
            var additionalVariants = new List<List<AnalysisData>>();

            var customerItemsLookup = Customer.Items.ToDictionary(q => q.Id);

            foreach (var variant in coreVariants)
            {
                foreach (var data in variant)
                {
                    var supplierItem = data.Item;
                    var customerItem = customerItemsLookup[supplierItem.Id];
                    var diffQty = supplierItem.Quantity - customerItem.Quantity;
                    if (diffQty < 0)
                    {
                        var remainingQty = Math.Abs(diffQty);
                        var variantsToFulfill = datas
                            .Where(q =>
                                q.Item.Id == supplierItem.Id &&
                                q.Item.InternalId != supplierItem.InternalId &&
                                q.Item.Quantity >= remainingQty).ToList();
                        if (variantsToFulfill.Any())
                        {
                            foreach (var variantToFulfill in variantsToFulfill)
                            {
                                var existingVariantsCopy = variant.ConvertAll(q => q.Copy());

                                var variantCopy = variantToFulfill.Copy();
                                variantCopy.Item.Quantity = remainingQty;

                                existingVariantsCopy.Add(variantCopy);
                                additionalVariants.Add(existingVariantsCopy);
                            }
                        }
                    }
                }
            }

            var allVariants = coreVariants.Union(additionalVariants).ToList();


            var variantsFilters = new List<IVariantsFilter>
            {
                new VariantsItemMustHavePriceFilter(),
                new VariantsItemQuantityFilter(),
                new VariantsSuppliersCountFilter(options.SuppliersCountOptions),
                new VariantsTotalValueFilter(options.TotalValueOptions)
            };

            foreach (var variantsFilter in variantsFilters)
            {
                allVariants = variantsFilter.Filter(allVariants, context);
            }

            if (!allVariants.Any()) return new AnalysisResult(new List<AnalysisData>());

            var resultData = allVariants.OrderBy(q => q.Sum(w => w.Item.TotalPrice)).Take(1).First();

            return new AnalysisResult(resultData);
        }

        private List<List<AnalysisData>> GenerateItemPairs(List<AnalysisData> datas)
        {
            var customerItemIds = Customer.Items.Select(q => q.Id).ToList();

            var results = new List<List<AnalysisData>>();

            foreach (var itemId in customerItemIds)
            {
                var items = datas.Where(q => q.Item.Id == itemId).ToList();

                results.Add(items);
            }

            return results;
        }
    }
}
