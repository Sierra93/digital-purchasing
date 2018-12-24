using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Analysis2.Enums;
using DigitalPurchasing.Core.Extensions;
using Newtonsoft.Json;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IAnalysisService
    {
        AnalysisDataVm GetData(Guid clId);
        AnalysisVariantOptions GetVariantData(Guid vId);
        AnalysisDataVm AddVariant(Guid clId);
        void SaveVariant(AnalysisSaveVariant variant);
        void DeleteVariant(Guid id);
        AnalysisDetails GetDetails(Guid clId);
    }

    public class AnalysisDetails
    {
        public class Item
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public decimal Quantity { get; set; }
            public string Uom { get; set; }
            public string Currency { get; set; }

            public List<Variant> Variants { get; set; } = new List<Variant>();
        }

        public class Variant
        {
            public string Name { get; set; }

            public Dictionary<Supplier, SupplierData> Suppliers { get; set; } = new Dictionary<Supplier, SupplierData>();
        }

        public class Supplier
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class SupplierData
        {
            public decimal TotalPrice { get; set; }
        }

        public List<Item> Items = new List<Item>();

        public decimal SumBySupplier(Variant variant, Supplier supplier)
            => Items.SelectMany(q => q.Variants)
                .Where(q => q.Name == variant.Name)
                .SelectMany(q => q.Suppliers)
                .Where(q => q.Key.Id == supplier.Id)
                .Sum(q => q.Value.TotalPrice);

        public decimal SumByVariant(Variant variant)
            => Items.SelectMany(q => q.Variants)
                .Where(q => q.Name == variant.Name)
                .SelectMany(q => q.Suppliers)
                .Sum(q => q.Value.TotalPrice);
    }

    public class AnalysisVariantOptions
    {
        public class Option
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }

        public List<Option> PaymentTermsOptions { get; set; }
            = Enum.GetValues(typeof(PaymentTerms))
                .OfType<PaymentTerms>()
                .Select(value => new Option { Text = value.GetDescription(), Value = ((int)value).ToString() })
                .ToList();
        public List<Option> DeliveryDateTermsOptions { get; set; }
            = Enum.GetValues(typeof(DeliveryDateTerms))
                .OfType<DeliveryDateTerms>()
                .Select(value => new Option { Text = value.GetDescription(), Value = ((int)value).ToString() })
                .ToList();
        public List<Option> DeliveryTermsOptions { get; set; }
            = Enum.GetValues(typeof(DeliveryTerms))
                .OfType<DeliveryTerms>()
                .Select(value => new Option { Text = value.GetDescription(), Value = ((int)value).ToString() })
                .ToList();

        public List<Option> SupplierCountOptions { get; set; }
            = new List<Option>
            {
                new Option { Text = "Без ограничений", Value = $"0:{(int)SupplierCountType.Any}" },
                new Option { Text = "Один", Value = $"1:{(int)SupplierCountType.Equal}" },
                new Option { Text = "Два", Value = $"2:{(int)SupplierCountType.Equal}" },
                new Option { Text = "Не более двух", Value = $"2:{(int)SupplierCountType.LessOrEqual}" },
                new Option { Text = "Не более трех", Value = $"3:{(int)SupplierCountType.LessOrEqual}" },
                new Option { Text = "Не более четырех", Value = $"4:{(int)SupplierCountType.LessOrEqual}" },
            };

        public PaymentTerms PaymentTerms { get; set; }
        public DeliveryDateTerms DeliveryDateTerms { get; set; }
        public DeliveryTerms DeliveryTerms { get; set; }
        public decimal? TotalValue { get; set; }

        [JsonProperty("supplierCount")]
        public string SupplierCountSelected => $"{SupplierCount}:{(int)SupplierCountType}";

        [JsonIgnore]
        public int SupplierCount { get; set; }
        [JsonIgnore]
        public SupplierCountType SupplierCountType { get; set; }
    }

    public class AnalysisSaveVariant
    {
        public Guid Id { get; set; }
        public PaymentTerms PaymentTerms { get; set; }
        public DeliveryDateTerms DeliveryDateTerms { get; set; }
        public DeliveryTerms DeliveryTerms { get; set; }
        public decimal? TotalValue { get; set; }
        public int SupplierCount { get; set; }
        public SupplierCountType SupplierCountType { get; set; }
    }

    public class AnalysisDataVm
    {
        public class Supplier
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public DeliveryTerms DeliveryTerms { get; set; }
            public PaymentTerms PaymentTerms { get; set; }

            public int PayWithinDays { get; set; }

            public DateTime? DeliveryDate { get; set; }

            public string DeliveryTermsStr => DeliveryTerms.GetDescription();
            public string PaymentTermsStr => PaymentTerms.GetDescription();
        }

        public class Variant
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public List<Result> Results { get; set; } = new List<Result>();
            public AnalysisVariantOptions Options { get; set; } = new AnalysisVariantOptions();
            public DateTime CreatedOn { get; set; }
        }

        public class Result
        {
            public Guid SupplierId { get; set; }
            public decimal Total { get; set; }
            public int Order { get; set; }
        }

        public DateTime? CustomerDeliveryDate { get; set; }

        public List<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public List<Variant> Variants { get; set; } = new List<Variant>();
    }
}
