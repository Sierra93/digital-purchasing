using System;
using System.Collections.Generic;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Models
{
    public class UnitsOfMeasurement : BaseModel, IHaveOwner
    {
        public string Name { get; set; }

        public Guid OwnerId { get; set; }
        public Company Owner { get; set; }

        public string NormalizedName { get; set; }

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
