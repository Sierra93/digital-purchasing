using System;
using System.Collections.Generic;
using DigitalPurchasing.Core.Enums;

namespace DigitalPurchasing.Models
{
    public class Root : BaseModelWithOwner
    {
        public RootStatus Status { get; set; }

        public PurchaseRequest PurchaseRequest { get; set; }
        public Guid? PurchaseRequestId { get; set; }

        public QuotationRequest QuotationRequest { get; set; }
        public Guid? QuotationRequestId { get; set; }

        public CompetitionList CompetitionList { get; set; }
        public Guid? CompetitionListId { get; set; }
    }
}
