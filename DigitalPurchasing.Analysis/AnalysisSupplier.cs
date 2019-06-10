using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Enums;

namespace DigitalPurchasing.Analysis
{
    public readonly struct AnalysisSupplier
    {
        public Guid SupplierInternalId { get; }
        public Guid SupplierId { get; }
        public Guid SupplierOfferId { get; }
        public DateTime? DeliveryDate { get; }
        public DeliveryTerms DeliveryTerms { get; }
        public PaymentTerms PaymentTerms { get; }
        public List<AnalysisSupplierItem> Items { get; }

        public AnalysisSupplier(
            Guid supplierId,
            Guid supplierOfferId,
            DateTime? deliveryDate,
            DeliveryTerms deliveryTerms,
            PaymentTerms paymentTerms,
            IEnumerable<AnalysisSupplierItem> items)
        {
            SupplierInternalId = Guid.NewGuid();
            SupplierId = supplierId;
            SupplierOfferId = supplierOfferId;
            DeliveryDate = deliveryDate;
            DeliveryTerms = deliveryTerms;
            PaymentTerms = paymentTerms;
            Items = items.ToList();
        }

        public AnalysisSupplier(
            Guid supplierId,
            Guid supplierOfferId,
            DateTime? deliveryDate,
            IEnumerable<AnalysisSupplierItem> items) : this(supplierId, supplierOfferId, deliveryDate,
            DeliveryTerms.NoRequirements, PaymentTerms.NoRequirements, items)
        {
        }
    }
}