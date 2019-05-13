using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IReceivedEmailService
    {
        bool IsProcessed(uint uid);
        void MarkProcessed(uint uid);
        Guid SaveRfqEmail(uint uid, Guid qrId, string subject, string body, string fromEmail,
            IReadOnlyList<(string fileName, string contentType, byte[] fileBytes)> attachments);
        RfqEmailVm GetRfqEmail(Guid emailId);
    }

    public class RfqEmailVm
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromEmail { get; set; }
        public Guid QuotationRequestId { get; set; }

        public IReadOnlyList<EmailAttachmentVm> Attachments { get; set; }
    }

    public class EmailAttachmentVm
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
    }
}
