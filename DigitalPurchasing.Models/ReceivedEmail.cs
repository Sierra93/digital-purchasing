using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public class ReceivedEmail : BaseModel
    {
        public uint UniqueId { get; set; }
        public bool IsProcessed { get; set; }
        public int ProcessingTries { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromEmail { get; set; }
        public DateTimeOffset MessageDate { get; set; }

        public ICollection<EmailAttachment> Attachments { get; set; }
    }
}
