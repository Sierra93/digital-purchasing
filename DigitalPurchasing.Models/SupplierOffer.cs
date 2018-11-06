using System;
using System.Collections.Generic;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Models
{
    public class SupplierOffer : BaseModelWithOwner
    {
        public int PublicId { get; set; }

        public Guid CompetitionListId { get; set; }
        public CompetitionList CompetitionList { get; set; }

        public Guid? UploadedDocumentId { get; set; }
        public UploadedDocument UploadedDocument { get; set; }

        public SupplierOfferStatus Status { get; set; }

        public string SupplierName { get; set; }
        public ICollection<SupplierOfferItem> Items = new List<SupplierOfferItem>();

        public Guid CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}
