using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public class File : BaseModel
    {
        public string FileName { get; set; }
        public byte[] Bytes { get; set; }
        public string ContentType { get; set; }
    }
}
