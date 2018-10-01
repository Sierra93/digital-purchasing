using System.ComponentModel.DataAnnotations;

namespace DigitalPurchasing.Models
{
    public abstract class Counter : BaseModelWithOwner
    {
        [ConcurrencyCheck]
        public int CurrentId { get; set; }
    }

    public class QRCounter : Counter
    {
    }
}
