using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Analysis2.Enums;

namespace DigitalPurchasing.Analysis2
{
    public class AnalysisCore
    {
        public Customer Customer { get; set; }
        public List<Supplier> Suppliers { get; set; } = new List<Supplier>();

        public AnalysisResult Run(AnalysisOptions options)
        {
            var suppliers = Suppliers;
            var customerItemIds = Customer.Items.Select(q => q.Id);

            if (options.DeliveryDate.HasValue)
            {
                switch (options.DeliveryDate.Value)
                {
                    case DeliveryDateType.Min:
                        var minDate = Suppliers.Where(q => q.Date.HasValue).Select(q => q.Date.Value).Min();
                        suppliers = suppliers.Where(q => q.Date.HasValue && q.Date == minDate).ToList();
                        break;
                    case DeliveryDateType.LessThanInRequest:
                        suppliers = suppliers.Where(q => q.Date.HasValue && q.Date <= Customer.Date).ToList();
                        break;
                }
            }

            if (options.DeliveryTerms.HasValue && options.DeliveryTerms.Value > 0)
            {
                suppliers = suppliers.Where(q => q.DeliveryTerms == options.DeliveryTerms.Value).ToList();
            }

            if (options.PaymentTerms.HasValue && options.PaymentTerms.Value > 0)
            {
                suppliers = suppliers.Where(q => q.PaymentTerms == options.PaymentTerms.Value).ToList();
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

            var allVariants = itemPairs.CartesianProduct();


            if (options.SupplierCount != null)
            {
                switch (options.SupplierCount.Type)
                {
                    case SupplierCountType.Equal:
                        allVariants = allVariants.Where(q =>
                            q.Select(w => w.Supplier).Distinct().Count() == options.SupplierCount.Count)
                             .ToList();
                        break;
                    case SupplierCountType.LessOrEqual:
                        allVariants = allVariants.Where(q =>
                            q.Select(w => w.Supplier).Distinct().Count() <= options.SupplierCount.Count)
                             .ToList();
                        break;
                }
            }

            if (options.TotalValue.HasValue)
            {
                allVariants = allVariants
                    .Where(q => q.Sum(w => w.Item.TotalPrice) <= options.TotalValue.Value)
                    .ToList();
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
