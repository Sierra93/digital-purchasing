using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public class ReceivedEmail : BaseModel
    {
        public uint UniqueId { get; set; }
        public bool IsProcessed { get; set; }
    }
}
