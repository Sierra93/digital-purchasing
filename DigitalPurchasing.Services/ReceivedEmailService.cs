using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;

namespace DigitalPurchasing.Services
{
    public class ReceivedEmailService : IReceivedEmailService
    {
        private readonly ApplicationDbContext _db;

        public ReceivedEmailService(ApplicationDbContext db)
            => _db = db;

        public bool IsProcessed(uint uid)
        {
            var email = GetByUid(uid);
            return email != null && email.IsProcessed;
        }

        public void MarkProcessed(uint uid)
        {
            var email = GetByUid(uid);
            if (email != null && !email.IsProcessed)
            {
                email.IsProcessed = true;
                _db.SaveChanges();
            }
        }

        private ReceivedEmail GetByUid(uint uid) => _db.ReceivedEmails.FirstOrDefault(q => q.UniqueId == uid);

        public Guid SaveRfqEmail(uint uid, Guid qrId, string subject, string body,
            string fromEmail, IReadOnlyList<(string fileName, string contentType, byte[] fileBytes)> attachments)
        {
            var email = GetByUid(uid);
            if (email == null)
            {
                email = new ReceivedRfqEmail
                {
                    UniqueId = uid,
                    IsProcessed = false,
                    Subject = subject,
                    Body = body,
                    FromEmail = fromEmail,
                    Attachments = attachments.Select(a => new EmailAttachment()
                    {
                        FileName = a.fileName,
                        Bytes = a.fileBytes,
                        ContentType = a.contentType
                    }).ToList(),
                    QuotationRequestId = qrId
                };
                _db.ReceivedEmails.Add(email);
                _db.SaveChanges();
            }

            return email.Id;
        }

        public RfqEmailVm GetRfqEmail(Guid emailId) => _db.ReceivedEmails.OfType<ReceivedRfqEmail>().FirstOrDefault(e => e.Id == emailId)?.Adapt<RfqEmailVm>();
    }
}
