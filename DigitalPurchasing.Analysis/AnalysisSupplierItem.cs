using System;

namespace DigitalPurchasing.Analysis
{
    public readonly struct AnalysisSupplierItem
    {
        public AnalysisSupplierItem(
            Guid nomenclatureId,
            decimal quantity,
            decimal price)
        {
            InternalId = Guid.NewGuid();
            NomenclatureId = nomenclatureId;
            Quantity = quantity;
            Price = price;
        }

        public Guid InternalId { get; }
        public Guid NomenclatureId { get; }

        public decimal Quantity { get; }
        public decimal Price { get; }
        public decimal TotalPrice => Quantity * Price;
    }
}