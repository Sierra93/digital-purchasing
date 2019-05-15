using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public class EmailAttachment : File
    {
        public ReceivedEmail ReceivedEmail { get; set; }
        public Guid ReceivedEmailId { get; set; }
    }
}
