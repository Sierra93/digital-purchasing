using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalPurchasing.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace DigitalPurchasing.Models.Identity
{
    public class User : IdentityUser<Guid>
    {
        public User() => Id = Guid.NewGuid();

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }

        public string JobTitle { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        #region Price reduction settings

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PRDiscountPercentage { get; set; }

        public SendPriceReductionTo SendPriceReductionTo { get; set; }

        public int RoundsCount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PriceReductionResponseHours { get; set; }

        #endregion

        [Column(TypeName = "decimal(18, 2)")]
        public decimal QuotationRequestResponseHours { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AutoCloseCLHours { get; set; } // todo: calc from RoundsCount * PriceReductionResponseHours
    }
}
