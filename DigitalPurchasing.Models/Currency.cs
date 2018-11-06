using System;

namespace DigitalPurchasing.Models
{
    public class Currency : BaseModel<Guid>
    {
        public string Name { get; set; }
    }
}
