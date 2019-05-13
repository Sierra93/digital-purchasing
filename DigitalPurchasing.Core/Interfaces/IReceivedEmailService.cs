using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IReceivedEmailService
    {
        bool IsProcessed(uint uid);
        void MarkProcessed(uint uid);
        Guid SaveRfqEmail(uint uid, Guid qrId, string subject, string body, string fromEmail, DateTimeOffset messageDate,
            IReadOnlyList<(string fileName, string contentType, byte[] fileBytes)> attachments);
        RfqEmailVm GetRfqEmail(Guid emailId);
        EmailAttachmentVm GetAttachment(Guid attachmentId);
        InboxIndexData GetData(Guid ownerId, bool unhandledSupplierOffersOnly,
            int page, int perPage, string sortField, bool sortAsc, string search);
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
        public Guid Id { get; set; }
        public string ContentType { get; set; }
    }

    public class InboxIndexData : BaseDataResponse<InboxIndexDataItem>
    {
    }

    public class InboxIndexAttachment
    {
        public string FileName { get; set; }
        public Guid Id { get; set; }
    }

    public class InboxIndexDataItem
    {
        public Guid Id { get; set; }
        public DateTimeOffset MessageDate { get; set; }
        public string Subject { get; set; }
        public string SupplierName { get; set; }
        public string Body { get; set; }
        public IReadOnlyList<InboxIndexAttachment> Attachments { get; set; } = new List<InboxIndexAttachment>();
    }
}
