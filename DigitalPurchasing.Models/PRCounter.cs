using System.ComponentModel.DataAnnotations;

namespace DigitalPurchasing.Models
{
    public class PRCounter : BaseModelWithOwner
    {
        [ConcurrencyCheck]
        public int CurrentId { get; set; }
    }
}
