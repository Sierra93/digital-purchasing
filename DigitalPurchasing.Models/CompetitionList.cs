using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Models
{
    public class CompetitionList : BaseModelWithOwner
    {
        public int PublicId { get; set; }

        public QuotationRequest QuotationRequest { get; set; }
        public Guid QuotationRequestId { get; set; }

        public ICollection<SupplierOffer> SupplierOffers { get; set; }

        public bool? IsClosed { get; set; }
        public DateTime AutomaticCloseDate { get; set; }
    }
}
