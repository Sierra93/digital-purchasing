using System.Collections.Generic;
using DigitalPurchasing.Models.Counters;
using DigitalPurchasing.Models.Identity;

namespace DigitalPurchasing.Models
{
    public class Company : BaseModel
    {
        public string Name { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<UnitsOfMeasurement> UnitsOfMeasurements { get; set; }
        public ICollection<NomenclatureCategory> NomenclatureCategories { get; set; }
        public ICollection<PRCounter> PRCounters { get; set; }

        public string InvitationCode { get; set; }
    }
}
