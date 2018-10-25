using System;

namespace DigitalPurchasing.Models
{
    public class CompetitionList : BaseModelWithOwner
    {
        public int PublicId { get; set; }

        public QuotationRequest QuotationRequest { get; set; }
        public Guid QuotationRequestId { get; set; }
    }
}
