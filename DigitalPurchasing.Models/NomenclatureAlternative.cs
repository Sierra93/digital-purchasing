using System;

namespace DigitalPurchasing.Models
{
    public class NomenclatureAlternative : BaseModelWithOwner
    {
        public Guid NomenclatureId { get; set; }
        public Nomenclature Nomenclature { get; set; }

        public string Name { get; set; }
        public string CustomerName { get; set; }
    }
}
