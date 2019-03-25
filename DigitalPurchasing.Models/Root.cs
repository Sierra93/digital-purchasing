using System;
using DigitalPurchasing.Core.Enums;

namespace DigitalPurchasing.Models
{
    public class Root : BaseModelWithOwner
    {
        public PurchaseRequest PurchaseRequest { get; set; }
        public Guid? PurchaseRequestId { get; set; }

        public QuotationRequest QuotationRequest { get; set; }
        public Guid? QuotationRequestId { get; set; }

        public CompetitionList CompetitionList { get; set; }
        public Guid? CompetitionListId { get; set; }
        public RootStatus Status { get; set; }
    }
}
