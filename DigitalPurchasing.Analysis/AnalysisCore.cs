using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Analysis.Filters;
using DigitalPurchasing.Analysis.Pipelines;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces.Analysis;

namespace DigitalPurchasing.Analysis
{
    public class AnalysisCore : IAnalysisCore
    {
        private readonly AnalysisCustomer _customer;
        private readonly IEnumerable<AnalysisSupplier> _suppliers;
        private readonly Dictionary<Guid, AnalysisCustomerItem> _customerItemsLookup;

        public AnalysisCore(AnalysisCustomer customer, IEnumerable<AnalysisSupplier> suppliers)
        {
            _customer = customer;
            _suppliers = suppliers.ToList();
            _customerItemsLookup = _customer.Items.ToDictionary(q => q.NomenclatureId);
        }

        public List<AnalysisResult> Run(params AnalysisVariantData[] variantDatas)
        {
            var results = new List<AnalysisResult>();

            foreach (var variantData in variantDatas)
            {
                var supplierPipeline = new SupplierPipeline();
                supplierPipeline.Register(
                    new SupplierDeliveryDateTermsFilter(variantData.DeliveryDateTermsOptions, _customer),
                    new SupplierDeliveryTermsFilter(variantData.DeliveryTermsOptions),
                    new SupplierPaymentTermsFilter(variantData.PaymentTermsOptions));

                var suppliers = supplierPipeline.Process(_suppliers).ToList();

                if (suppliers.Count() == 0)
                {
                    results.Add(AnalysisResult.Empty(variantData.Id));
                    continue;
                }

                var minVariant = new Dictionary<Guid, decimal>();

                foreach (var customerItem in _customerItemsLookup)
                {
                    var items = suppliers
                        .SelectMany(q => q.Items.Where(w => w.NomenclatureId == customerItem.Key && w.Quantity >= customerItem.Value.Quantity));

                    if (items.Any())
                    {
                        minVariant.Add(customerItem.Key, items.Min(q => q.TotalPrice));
                    }
                    else
                    {
                        results.Add(AnalysisResult.Empty(variantData.Id));
                        continue;
                    }
                }

                var suppliersCount = suppliers.Count;
                var suppliersCountOptions = variantData.SuppliersCountOptions;
                var suppliersIndexes = Enumerable.Range(0, suppliersCount);
                var suppliersIndexesCombinations = suppliersIndexes.Combinations();

                switch (suppliersCountOptions.Type)
                {
                    case SupplierCountType.Any:
                        suppliersIndexesCombinations = suppliersIndexesCombinations.Where(q => q.Count() >= 1);
                        break;
                    case SupplierCountType.Equal:
                        suppliersIndexesCombinations = suppliersIndexesCombinations.Where(q => q.Count() == suppliersCountOptions.Count);
                        break;
                    case SupplierCountType.LessOrEqual:
                        suppliersIndexesCombinations = suppliersIndexesCombinations.Where(q => q.Count() >= 1 && q.Count() <= suppliersCountOptions.Count);
                        break;
                }

                var suppliersScores = new List<(List<int> Indexes, decimal Score)>();

                foreach (var suppliersIndexesCombination in suppliersIndexesCombinations)
                {
                    decimal score = 1;

                    foreach (var customerItemLooku in _customerItemsLookup)
                    {
                        var nomenclatureId = customerItemLooku.Key;
                        var customerQuantity = customerItemLooku.Value.Quantity;

                        var items = suppliers
                            .Where(q => suppliersIndexesCombination.Contains(suppliers.IndexOf(q)))
                            .SelectMany(q => q.Items.Where(i => i.NomenclatureId == nomenclatureId && i.Quantity >= customerQuantity));

                        if (items.Any())
                        {
                            var scoreMod = minVariant[nomenclatureId] / items.Min(q => q.TotalPrice);
                            score *= scoreMod;
                        }
                        else
                        {
                            score = 0;
                        }
                    }

                    suppliersScores.Add((Indexes: suppliersIndexesCombination.ToList(), Score: score));
                }

                var bestCombination = suppliersScores.OrderByDescending(q => q.Score).First();

                if (bestCombination.Score == 0)
                {
                    results.Add(AnalysisResult.Empty(variantData.Id));
                    continue;
                }

                var bestCombinationSuppliers = suppliers
                        .Where(q => bestCombination.Indexes.Contains(suppliers.IndexOf(q)));

                var bestDatas = new List<AnalysisResultData>();

                foreach (var customerItemLookup in _customerItemsLookup)
                {
                    var nomenclatureId = customerItemLookup.Key;

                    var datas = bestCombinationSuppliers
                        .Select(q => {
                            var supplier = q;
                            var supplierItem = q.Items
                                .Where(i => i.NomenclatureId == nomenclatureId)
                                .Cast<AnalysisSupplierItem?>()
                                .FirstOrDefault();

                            if (!supplierItem.HasValue)
                            {
                                return default;                                
                            }

                            var customerQuantity = _customerItemsLookup[nomenclatureId].Quantity;
                            var resultData = new AnalysisResultData(
                                supplier.SupplierId,
                                supplier.SupplierOfferId,
                                supplierItem.Value.NomenclatureId,
                                supplierItem.Value.Price,
                                supplierItem.Value.Quantity,
                                customerQuantity);

                            return resultData;
                        });

                    if (datas.Any(q => !q.Equals(default(AnalysisResultData))))
                    {
                        var data = datas.Where(q => !q.Equals(default(AnalysisResultData))).Aggregate((min, x) => x.TotalPrice < min.TotalPrice ? x : min);
                        bestDatas.Add(data);
                    }
                    else
                    {
                        // only all positions
                        results.Add(AnalysisResult.Empty(variantData.Id));
                        continue;
                    }                        
                }

                // only all positions
                if (bestDatas.Count < _customer.Items.Count)
                {
                    results.Add(AnalysisResult.Empty(variantData.Id));
                    continue;
                }

                // total price filter
                if (variantData.TotalValueOptions?.Value > 0 && bestDatas.Sum(q => q.TotalPrice) > variantData.TotalValueOptions.Value)
                {
                    results.Add(AnalysisResult.Empty(variantData.Id));
                    continue;
                }
                
                results.Add(new AnalysisResult(variantData.Id, bestDatas));
            }

            return results;
        }

        public AnalysisResult Run(AnalysisVariantData variantData) => Run(new[] {variantData}).First();
    }
}
