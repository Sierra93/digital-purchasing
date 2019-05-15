using System;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalPurchasing.Models.Identity;

namespace DigitalPurchasing.Models.SSR
{
    public class SSReport : BaseModelWithOwner
    {
        public Root Root { get; set; }
        public Guid RootId { get; set; }

        public User User { get; set; }
        public Guid UserId { get; set; }

        public DateTime CLCreatedOn { get; set; }
        public int CLNumber { get; set; }

        public int SelectedVariantNumber { get; set; }

        [Column(TypeName = "decimal(38, 17)")]
        public decimal SelectedVariantTotalPrice { get; set; }
    }
}
