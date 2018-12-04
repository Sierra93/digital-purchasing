using System;
using System.Collections.Generic;
using System.Text;

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
        }

        public class Variant
        {
            public List<Result> Results { get; set; } = new List<Result>();
        }

        public class Result
        {
            public Guid SupplierId { get; set; }
            public decimal Total { get; set; }
            public int Order { get; set; }
        }

        public List<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public List<Variant> Variants { get; set; } = new List<Variant>();
    }


}
