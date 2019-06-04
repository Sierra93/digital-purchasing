using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public abstract class AppNGram : BaseModel
    {
        public byte N { get; set; }
        public string Gram { get; set; }
    }
}
