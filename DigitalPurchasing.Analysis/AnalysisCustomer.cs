using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Analysis
{
    public readonly struct AnalysisCustomer
    {
        public Guid CustomerInternalId { get; }
        public Guid CustomerId { get; }
        public Guid PurchaseRequestId { get; }
        public DateTime? DeliveryDate { get; }
        public List<AnalysisCustomerItem> Items { get; }
        
        public AnalysisCustomer(
            Guid customerId,
            Guid purchaseRequestId,
            DateTime? deliveryDate,
            IEnumerable<AnalysisCustomerItem> items)
        {
            CustomerInternalId = Guid.NewGuid();
            CustomerId = customerId;
            PurchaseRequestId = purchaseRequestId;
            DeliveryDate = deliveryDate;
            Items = items.ToList();
        }
    }
}