using System;
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
    }
}
