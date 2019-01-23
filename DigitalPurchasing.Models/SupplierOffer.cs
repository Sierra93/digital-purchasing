using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Models
{
    public class SupplierOffer : BaseModelWithOwner
    {
        public string SupplierName {get; set; }

        public Guid? SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public int PublicId { get; set; }

        public Guid CompetitionListId { get; set; }
        public CompetitionList CompetitionList { get; set; }

        public Guid? UploadedDocumentId { get; set; }
        public UploadedDocument UploadedDocument { get; set; }

        public SupplierOfferStatus Status { get; set; }

        public ICollection<SupplierOfferItem> Items = new List<SupplierOfferItem>();

        public Guid CurrencyId { get; set; }
        public Currency Currency { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal DeliveryCost { get; set; }

        #region Terms

        public DateTime ConfirmationDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public int PriceFixedForDays { get; set; }
        public int ReservedForDays { get; set; }
        public int DeliveryAfterConfirmationDays { get; set; }

        public DeliveryTerms DeliveryTerms { get; set; }

        public PaymentTerms PaymentTerms { get; set; }

        public int PayWithinDays { get; set; }

        #endregion
    }
}
