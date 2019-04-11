using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Analysis2.Filters;

namespace DigitalPurchasing.Analysis2
{
    public class AnalysisCore
    {
        private IAnalysisContext _context;
        private Dictionary<Guid, CustomerItem> _customerItemsLookup;
        private AnalysisCustomer _analysisCustomer;
        private List<AnalysisSupplier> _analysisSuppliers;

        public AnalysisCore(AnalysisCustomer analysisCustomer, List<AnalysisSupplier> analysisSuppliers)
        {
            _analysisCustomer = analysisCustomer;
            _analysisSuppliers = analysisSuppliers;
            _context = new AnalysisContext { Customer = analysisCustomer };
            _customerItemsLookup = analysisCustomer.Items.ToDictionary(q => q.Id);
        }

        public AnalysisResult Run(AnalysisCoreVariant options)
        {
            var suppliers = _analysisSuppliers;

            foreach (var filter in GetSuppliersFilter(options))
            {
                suppliers = filter.Filter(suppliers, _context);
            }

            return Process(_analysisCustomer, suppliers, options);
        }

        private List<ISuppliersFilter> GetSuppliersFilter(AnalysisCoreVariant options)
        {
            var suppliersFilters = new List<ISuppliersFilter>
            {
                new SupplierDeliveryTermsFilter(options.DeliveryTermsOptions),
                new SupplierPaymentTermsFilter(options.PaymentTermsOptions),
                new SupplierDeliveryDateTermsFilter(options.DeliveryDateTermsOptions)
            };
            return suppliersFilters;
        }

        private List<IVariantsFilter> GetVariantsFilters(AnalysisCoreVariant options)
        {
            var variantsFilters = new List<IVariantsFilter>
            {
                new VariantsItemMustHavePriceFilter(),
                new VariantsItemQuantityFilter(),
                new VariantsSuppliersCountFilter(options.SuppliersCountOptions),
                new VariantsTotalValueFilter(options.TotalValueOptions)
            };
            return variantsFilters.OrderBy(q => q.Order).ToList();
        }



        private AnalysisResult Process(AnalysisCustomer analysisCustomer, List<AnalysisSupplier> suppliers, AnalysisCoreVariant options)
        {
            var sw = new Stopwatch();
            
            var allVariants = GenerateAllVariants(suppliers); 
            
            foreach (var variantsFilter in GetVariantsFilters(options))
            {
                sw.Restart();
                allVariants = variantsFilter.Filter(allVariants, _context);
                sw.Stop();
                Console.WriteLine($"---- {variantsFilter.GetType().Name} ---- {sw.Elapsed.TotalMilliseconds} ms");
            }

            if (!allVariants.Any()) return AnalysisResult.Empty(options.Id);

            var resultData = allVariants.OrderBy(q => q.Sum(w => w.Item.TotalPrice)).Take(1).First();

            return new AnalysisResult(options.Id, resultData);
        }
        
        public List<AnalysisResult> Run(params AnalysisCoreVariant[] options)
        {
            var sw = new Stopwatch();

            var suppliersIdsByOptions = new Dictionary<Guid, List<Guid>>();

            foreach (var option in options)
            {
                var variantSuppliers = _analysisSuppliers;

                var suppliersFilters = new List<ISuppliersFilter>
                {
                    new SupplierDeliveryTermsFilter(option.DeliveryTermsOptions),
                    new SupplierPaymentTermsFilter(option.PaymentTermsOptions),
                    new SupplierDeliveryDateTermsFilter(option.DeliveryDateTermsOptions)
                };

                foreach (var filter in suppliersFilters)
                {
                    variantSuppliers = filter.Filter(variantSuppliers, _context);
                }

                suppliersIdsByOptions.Add(option.Id, variantSuppliers.Select(q => q.Id).ToList());
            }

            var uniqueSuppliersSet = suppliersIdsByOptions.Values
                .Select(x => new HashSet<Guid>(x))
                .Distinct(HashSet<Guid>.CreateSetComparer())
                .ToList();

            var results = new List<AnalysisResult>();

            foreach (var suppliersIds in uniqueSuppliersSet)
            {
                var suppliers = _analysisSuppliers.Where(q => suppliersIds.Contains(q.Id)).ToList();

                var allVariants = GenerateAllVariants(suppliers);
                
                var optionIdsWSuppliers = suppliersIdsByOptions
                    .Where(q => !q.Value.Except(suppliersIds.ToList()).Any())
                    .Select(q => q.Key)
                    .ToList();

                foreach (var optionId in optionIdsWSuppliers)
                {
                    var variants = allVariants;
                    foreach (var variantsFilter in GetVariantsFilters(options.First(q => q.Id == optionId)))
                    {
                        sw.Restart();
                        variants = variantsFilter.Filter(variants, _context);
                        sw.Stop();
                        Console.WriteLine($"---- {variantsFilter.GetType().Name} ---- {sw.Elapsed.TotalMilliseconds} ms");
                    }

                    if (variants.Any())
                    {
                        var resultData = variants.OrderBy(q => q.Sum(w => w.Item.TotalPrice)).Take(1).First();
                        results.Add(new AnalysisResult(optionId, resultData));
                    }
                    else
                    {
                        results.Add(AnalysisResult.Empty(optionId));
                    }
                }
            }

            return results;
        }

        private List<List<AnalysisData>> GenerateAllVariants(List<AnalysisSupplier> suppliers)
        {
            var datas = new List<AnalysisData>();
            foreach (var supplier in suppliers)
            {
                var items = supplier.Items.Where(q => _customerItemsLookup.ContainsKey(q.Id));
                foreach (var item in items)
                {
                    datas.Add(new AnalysisData { SupplierId = supplier.Id, Item = item, CustomerQuantity = _customerItemsLookup[item.Id].Quantity  });
                }
            }

            var sw = new Stopwatch();
            var itemPairs = GenerateItemPairs(_analysisCustomer.Items, datas);

            var coreVariants = itemPairs.CartesianProduct();
            var additionalVariants = new List<List<AnalysisData>>();

            sw.Restart();

            Parallel.ForEach(coreVariants, (variant) =>
            {
                bool HavePartialQuantity(AnalysisData w) => ( w.Item.Quantity - _customerItemsLookup[w.Item.Id].Quantity < 0 );

                if (variant.Any(HavePartialQuantity))
                {
                    foreach (var data in variant.Where(HavePartialQuantity))
                    {
                        var supplierItem = data.Item;
                        var customerItem = _customerItemsLookup[supplierItem.Id];
                        var diffQty = supplierItem.Quantity - customerItem.Quantity;
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
            });
            
            sw.Stop();
            Console.WriteLine($"---- additionalVariants ---- {sw.Elapsed.TotalMilliseconds} ms");

            return coreVariants.Union(additionalVariants).ToList();
        }

        private List<List<AnalysisData>> GenerateItemPairs(List<CustomerItem> customerItems, List<AnalysisData> datas)
        {
            var ids = new List<Guid>(customerItems.Count);
            ids.AddRange(customerItems.Select(q => q.Id));
            
            var results = new List<List<AnalysisData>>();

            foreach (var itemId in ids)
            {
                var items = datas.Where(q => q.Item.Id == itemId).ToList();

                results.Add(items);
            }

            return results;
        }
    }
}
