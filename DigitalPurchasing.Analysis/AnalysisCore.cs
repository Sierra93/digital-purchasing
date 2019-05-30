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

                //var totalCustomerQuantity = _customerItemsLookup.Sum(q => q.)

                var minVariant = new Dictionary<Guid, (decimal TotalPrice, decimal Price)>();

                foreach (var customerItem in _customerItemsLookup)
                {
                    var customerQuantity = customerItem.Value.Quantity;
                    var supplierItems = suppliers.SelectMany(q => q.Items.Where(w => w.NomenclatureId == customerItem.Key));
                    var itemsWEnoughQuality = supplierItems.Any(q => q.Quantity >= customerQuantity);
                    var maxQuantity = supplierItems.Sum(q => q.Quantity);
                    if (itemsWEnoughQuality || maxQuantity > customerQuantity)
                    {
                        minVariant.Add(customerItem.Key, (TotalPrice: supplierItems.Min(q => q.TotalPrice), Price: supplierItems.Min(q => q.Price)));
                    }
                    else
                    {
                        results.Add(AnalysisResult.Empty(variantData.Id));
                        break;
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

                        var suppliersItems = suppliers
                            .Where(q => suppliersIndexesCombination.Contains(suppliers.IndexOf(q)))
                            .SelectMany(q => q.Items.Where(i => i.NomenclatureId == nomenclatureId));

                        var itemsWEnoughQuality = suppliersItems.Where(q => q.Quantity >= customerQuantity);

                        if (itemsWEnoughQuality.Any())
                        {
                            var scoreMod = minVariant[nomenclatureId].TotalPrice / itemsWEnoughQuality.Min(q => q.TotalPrice);
                            // todo: apply according to quantities
                            score *= scoreMod;                            
                        }
                        else
                        {
                            var maxQuantity = suppliersItems.Sum(q => q.Quantity);
                            var isPossibleToFillPosition = maxQuantity >= customerQuantity;
                            if (isPossibleToFillPosition)
                            {
                                var scoreMod = 0m;
                                var currentQuantity = 0m;
                                var minPriceData = minVariant[nomenclatureId];
                                foreach (var item in suppliersItems.OrderBy(q => q.Price).ThenByDescending(q => q.Quantity))
                                {
                                    var remainingQuantity = Math.Abs(customerQuantity - currentQuantity);
                                    var quantityToAdd = item.Quantity > remainingQuantity ? remainingQuantity : item.Quantity;
                                    currentQuantity += quantityToAdd;
                                    scoreMod += (quantityToAdd / customerQuantity) * minPriceData.Price / item.Price;
                                    if (currentQuantity == customerQuantity) break;
                                }
                                // todo: apply according to quantities
                                score *= scoreMod;
                            }
                            else
                            {
                                score = 0;
                            }                            
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
                    var customerQuantity = _customerItemsLookup[nomenclatureId].Quantity;

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
                                                        
                            var resultData = new AnalysisResultData(
                                supplier.SupplierId,
                                supplier.SupplierOfferId,
                                supplierItem.Value.NomenclatureId,
                                supplierItem.Value.Price,
                                supplierItem.Value.Quantity,
                                customerQuantity);

                            return resultData;
                        });

                    datas = datas.Where(q => !q.Equals(default(AnalysisResultData)));

                    if (datas.Any())
                    {
                        // single supplier
                        AnalysisResultData singleSupplierData = default;
                        if (datas.Any(q => q.Quantity >= q.CustomerQuantity))
                        {
                            singleSupplierData = datas
                                .Where(q => q.Quantity >= q.CustomerQuantity)
                                .Aggregate((min, x) => x.TotalPrice < min.TotalPrice ? x : min);
                        }

                        // multiple suppliers
                        var multipleSuppliersData = new List<AnalysisResultData>();                        
                        if (datas.Any(q => q.Quantity < q.CustomerQuantity))
                        {
                            var orderedDatas = datas.OrderBy(q => q.Price).ThenByDescending(q => q.Quantity);
                            var currentQuantity = 0m;
                            foreach (var item in orderedDatas)
                            {
                                var remainingQuantity = Math.Abs(customerQuantity - currentQuantity);
                                var quantityToAdd = item.Quantity > remainingQuantity ? remainingQuantity : item.Quantity;
                                currentQuantity += quantityToAdd;
                                multipleSuppliersData.Add(new AnalysisResultData(
                                    item.SupplierId,
                                    item.SupplierOfferId,
                                    item.NomenclatureId,
                                    item.Price,
                                    quantityToAdd,
                                    item.CustomerQuantity));

                                if (currentQuantity == customerQuantity) break;
                            }
                        }

                        if (!singleSupplierData.Equals(default(AnalysisResultData)) && multipleSuppliersData.Any())
                        {
                            if (singleSupplierData.TotalPrice <= multipleSuppliersData.Sum(q => q.TotalPrice))
                            {
                                bestDatas.Add(singleSupplierData);                                
                            }
                            else
                            {
                                bestDatas.AddRange(multipleSuppliersData);
                            }
                            continue;
                        }

                        if (multipleSuppliersData.Any())
                        {
                            bestDatas.AddRange(multipleSuppliersData);
                            continue;
                        }

                        bestDatas.Add(singleSupplierData);
                    }
                    else
                    {
                        // only all positions
                        results.Add(AnalysisResult.Empty(variantData.Id));
                        break;
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