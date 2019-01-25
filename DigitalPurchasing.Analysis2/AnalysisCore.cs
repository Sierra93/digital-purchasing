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

        public AnalysisResult Run(AnalysisOptions options)
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

            CalculateScores(datas);
            var itemPairs = GenerateItemPairs(datas);

            var coreVariants = itemPairs.CartesianProduct();
            var additionalVariants = new List<List<AnalysisData>>();

            foreach (var variant in coreVariants)
            {
                foreach (var data in variant)
                {
                    var supplierItem = data.Item;
                    var customerItem = Customer.Items.Find(q => q.Id == supplierItem.Id);
                    var diffQty = supplierItem.Quantity - customerItem.Quantity;
                    if (diffQty < 0)
                    {
                        var remainingQty = Math.Abs(diffQty);
                        var variantsToFullFill = datas
                            .Where(q =>
                                q.Item.Id == supplierItem.Id &&
                                q.Item.InternalId != supplierItem.InternalId &&
                                q.Item.Quantity >= remainingQty).ToList();
                        if (variantsToFullFill.Any())
                        {
                            foreach (var variantToFillFull in variantsToFullFill)
                            {
                                variantToFillFull.Item.Quantity = remainingQty;
                                var newVariant = variant.Select(q => new AnalysisData{ Item = q.Item, Score = q.Score, Supplier = q.Supplier }).ToList();
                                newVariant.Add(variantToFillFull);
                                additionalVariants.Add(newVariant);
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

            var resultData = allVariants.OrderByDescending(q => q.Sum(w => w.Score)).Take(1).First();

            return new AnalysisResult(resultData);
        }

        private void CalculateScores(List<AnalysisData> datas)
        {
            var customerItemIds = Customer.Items.Select(q => q.Id).ToList();

            foreach (var itemId in customerItemIds)
            {
                var items = datas.Where(q => q.Item.Id == itemId).Select(q => q.Item).ToList();
                if (items.Any())
                {
                    if (items.Count == 1)
                    {
                        SetScore(datas, items[0].InternalId, 1);
                    }
                    else
                    {
                        var itemWMinPrice = items.OrderBy(q => q.Price).First();
                        SetScore(datas, itemWMinPrice.InternalId, 1);

                        foreach (var item in items.Where(q => q.InternalId != itemWMinPrice.InternalId))
                        {
                            var score = itemWMinPrice.Price / item.Price;
                            SetScore(datas, item.InternalId, score);
                        }
                    }
                }
            }
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

        private void SetScore(List<AnalysisData> datas, Guid internalId, decimal score) => datas.Find(q => q.Item.InternalId == internalId).Score = score;
    }
}
