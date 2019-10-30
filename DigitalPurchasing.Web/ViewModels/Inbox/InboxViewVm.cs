using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPurchasing.Web.ViewModels.Inbox
{
    public class InboxViewVm
    {
        public sealed class EmailAttachment
        {
            public Guid Id { get; set; }
            public string FileName { get; set; }
        }

        public string SupplierName { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public DateTime EmailDate { get; set; }
        public IReadOnlyList<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
        public string EmailFrom { get; set; }
    }
}
