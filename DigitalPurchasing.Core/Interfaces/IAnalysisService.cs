using System;
using System.Collections.Generic;
using System.Text;
using DigitalPurchasing.Core.Extensions;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IAnalysisService
    {
        AnalysisDataVm GetData(Guid clId);
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
