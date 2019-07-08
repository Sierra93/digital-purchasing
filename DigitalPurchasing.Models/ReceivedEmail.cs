using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Models
{
    public class ReceivedEmail : BaseModel
    {
        public Guid? OwnerId { get; set; }
        public Company Owner { get; set; }

        public uint UniqueId { get; set; }
        public bool IsProcessed { get; set; }
        public int ProcessingTries { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromEmail { get; set; }
        public DateTimeOffset MessageDate { get; set; }

        public ICollection<EmailAttachment> Attachments { get; set; }
        public string ToEmail { get; set; }
    }
}
