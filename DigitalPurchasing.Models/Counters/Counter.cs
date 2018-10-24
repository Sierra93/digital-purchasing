using System.ComponentModel.DataAnnotations;

namespace DigitalPurchasing.Models.Counters
{
    public abstract class Counter : BaseModelWithOwner
    {
        [ConcurrencyCheck]
        public int CurrentId { get; set; }
    }
}
