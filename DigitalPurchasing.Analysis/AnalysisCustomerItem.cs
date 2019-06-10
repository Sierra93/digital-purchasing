using System;

namespace DigitalPurchasing.Analysis
{
    public readonly struct AnalysisCustomerItem
    {
        public AnalysisCustomerItem(
            Guid nomenclatureId,
            decimal quantity)
        {
            InternalId = Guid.NewGuid();
            NomenclatureId = nomenclatureId;
            Quantity = quantity;
        }

        public Guid InternalId { get; }
        public Guid NomenclatureId { get; }

        public decimal Quantity { get; }
    }
}