using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Analysis.Filters;
using DigitalPurchasing.Analysis.Pipelines;
using DigitalPurchasing.Core.Enums;
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
            _suppliers = suppliers;
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

                var suppliersCountOptions = variantData.SuppliersCountOptions;

                if (suppliersCountOptions.Type != SupplierCountType.Any)
                {
                    if (suppliersCountOptions.Type == SupplierCountType.LessOrEqual
                        || (suppliersCountOptions.Type == SupplierCountType.Equal && suppliersCountOptions.Count == 1))
                    {
                        suppliers = suppliers
                            .OrderBy(q => q.Items.Sum(w => w.TotalPrice))
                            .Take(suppliersCountOptions.Count)
                            .ToList();
                    }
                }

                var datas = new List<AnalysisResultData>();
                foreach (var supplier in suppliers)
                {
                    var items = supplier.Items.Where(q => _customerItemsLookup.ContainsKey(q.NomenclatureId));
                    foreach (var item in items)
                    {
                        var customerQuantity = _customerItemsLookup[item.NomenclatureId].Quantity;
                        datas.Add(new AnalysisResultData(
                            supplier.SupplierId,
                            supplier.SupplierOfferId,
                            item.NomenclatureId,
                            item.Price,
                            item.Quantity,
                            customerQuantity));
                    }
                }

                var datasByNomId = datas
                        .GroupBy(q => q.NomenclatureId)
                        .ToDictionary(k => k.Key, v => v)
                        .OrderByDescending(q => q.Value.Average(w => w.TotalPrice))
                        .ToDictionary(k => k.Key, v => v.Value.ToList());

                var bestDatas = new List<AnalysisResultData>();

                foreach (var dataByNomId in datasByNomId)
                {
                    var validDatas = dataByNomId.Value.Where(q => q.Quantity >= q.CustomerQuantity && q.TotalPrice > 0).ToList();
                    if (validDatas.Any())
                    {
                        var data = validDatas.Aggregate((min, x) => x.TotalPrice < min.TotalPrice ? x : min);
                        bestDatas.Add(data);
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
