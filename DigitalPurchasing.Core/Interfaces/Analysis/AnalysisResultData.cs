using System;

namespace DigitalPurchasing.Core.Interfaces.Analysis
{
    public readonly struct AnalysisResultData
    {
        public Guid InternalId { get; }
        public Guid SupplierId { get; }
        public Guid SupplierOfferId { get; }
        public Guid NomenclatureId { get; }

        public decimal Price { get; }
        public decimal Quantity { get; }

        public decimal TotalPrice => Quantity * Price;

        public decimal CustomerQuantity { get; }

        public AnalysisResultData(
            Guid supplierId,
            Guid supplierOfferId,
            Guid nomenclatureId,
            decimal price,
            decimal quantity,
            decimal customerQuantity)
        {
            InternalId = Guid.NewGuid();
            SupplierId = supplierId;
            SupplierOfferId = supplierOfferId;
            NomenclatureId = nomenclatureId;
            Price = price;
            Quantity = quantity;
            CustomerQuantity = customerQuantity;
        }
    }
}
