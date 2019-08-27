using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Models
{
    public class UomAutocomplete : IHaveOwner
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public Guid OwnerId { get; set; }
        public string AlternativeName { get; set; }
        public string NormalizedAlternativeName { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class UomAlternativeName
    {
        public string Name { get; set; }
        public string NormalizedName => Name.CustomNormalize();
    }

    public class UomJsonData
    {
        public List<UomAlternativeName> AlternativeNames { get; set; } = new List<UomAlternativeName>();
    }
    
    public class UnitsOfMeasurement : BaseModel, IHaveOwner
    {
        public string Name { get; set; }

        public Guid OwnerId { get; set; }
        public Company Owner { get; set; }

        public string NormalizedName { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal? Quantity { get; set; }

        public UomJsonData Json { get; set; } = new UomJsonData();

        public ICollection<Nomenclature> BatchNomenclatures { get; set; }
        public ICollection<Nomenclature> MassNomenclatures { get; set; }
        public ICollection<Nomenclature> ResourceNomenclatures { get; set; }
        public ICollection<Nomenclature> ResourceBatchNomenclatures { get; set; }

        public ICollection<NomenclatureAlternative> BatchNomenclatureAlternatives { get; set; }
        public ICollection<NomenclatureAlternative> MassNomenclatureAlternatives { get; set; }
        public ICollection<NomenclatureAlternative> ResourceNomenclatureAlternatives { get; set; }
        public ICollection<NomenclatureAlternative> ResourceBatchNomenclatureAlternatives { get; set; }

        public ICollection<UomConversionRate> FromConversionRates { get; set; }
        public ICollection<UomConversionRate> ToConversionRates { get; set; }

        public ICollection<PurchaseRequestItem> PurchasingRequestItems { get; set; }
        public bool IsDeleted { get; set; }
    }
}
