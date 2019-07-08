using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IReceivedEmailService
    {
        bool IsProcessed(uint uid);
        void MarkProcessed(uint uid);
        void IncProcessingTries(uint uid);

        bool IsSaved(uint uid);

        EmailStatus GetStatus(uint uid);

        Guid SaveEmail(uint uid, string subject, string body,
            string fromEmail, string toEmail, DateTimeOffset messageDate,
            IReadOnlyList<(string fileName, string contentType, byte[] fileBytes)> attachments,
            Guid? ownerId = null);

        EmailDto GetEmail(Guid emailId, bool includeAttachments = true);

        EmailAttachmentDto GetAttachment(Guid attachmentId);
        InboxIndexData GetData(Guid ownerId, bool unhandledSupplierOffersOnly,
            int page, int perPage, string sortField, bool sortAsc, string search);
    }

    public readonly struct EmailStatus
    {
        public EmailStatus(uint uid, Guid emailId, bool isSaved, bool isProcessed)
        {
            Uid = uid;
            EmailId = emailId;
            IsSaved = isSaved;
            IsProcessed = isProcessed;
        }

        public uint Uid { get; }
        public Guid EmailId { get; }
        public bool IsProcessed { get; }
        public bool IsSaved { get; }
    }

    public class EmailDto
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromEmail { get; set; }
        public Guid QuotationRequestId { get; set; }
        public int ProcessingTries { get; set; }
        public DateTimeOffset MessageDate { get; set; }

        public IReadOnlyList<EmailAttachmentDto> Attachments { get; set; }
    }

    public class EmailAttachmentDto
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
