using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class ReceivedEmailService : IReceivedEmailService
    {
        private readonly ApplicationDbContext _db;
        private readonly ISupplierService _supplierService;

        public ReceivedEmailService(
            ApplicationDbContext db,
            ISupplierService supplierService)
        {
            _db = db;
            _supplierService = supplierService;
        }

        public bool IsProcessed(uint uid)
        {
            var email = GetByUid(uid);
            return email != null && email.IsProcessed;
        }

        public void IncProcessingTries(uint uid)
        {
            var email = GetByUid(uid);
            if (email != null)
            {
                email.ProcessingTries++;
                _db.SaveChanges();
            }
        }

        public bool IsSaved(uint uid) => _db.ReceivedEmails.Any(q => q.UniqueId == uid);

        public EmailStatus GetStatus(uint uid)
        {
            var email = _db.ReceivedEmails.FirstOrDefault(q => q.UniqueId == uid);
            if (email == null) return new EmailStatus(uid, Guid.Empty,  false, false);
            return email.IsProcessed
                ? new EmailStatus(uid, email.Id, true, true)
                : new EmailStatus(uid, email.Id, true, false);
        }

        public void MarkProcessed(uint uid)
        {
            var email = GetByUid(uid);
            if (email != null && !email.IsProcessed)
            {
                email.IsProcessed = true;
                email.ProcessingTries++;
                _db.SaveChanges();
            }
        }

        private ReceivedEmail GetByUid(uint uid) => _db.ReceivedEmails.FirstOrDefault(q => q.UniqueId == uid);

        public Guid SaveEmail(uint uid, string subject, string body,
            string fromEmail, string toEmail, DateTimeOffset messageDate,
            IReadOnlyList<(string fileName, string contentType, byte[] fileBytes)> attachments, Guid? ownerId = null)
        {
            var email = GetByUid(uid);
            if (email != null) return email.Id;

            email = new ReceivedEmail
            {
                OwnerId = ownerId,
                UniqueId = uid,
                IsProcessed = false,
                Subject = subject,
                Body = body,
                FromEmail = fromEmail,
                ToEmail = toEmail,
                MessageDate = messageDate,
                Attachments = attachments.Select(a => new EmailAttachment
                {
                    FileName = a.fileName,
                    Bytes = a.fileBytes,
                    ContentType = a.contentType
                }).ToList()
            };

            _db.ReceivedEmails.Add(email);
            _db.SaveChanges();

            return email.Id;
        }

        public EmailDto GetEmail(Guid emailId, bool includeAttachments = true)
        {
            var qry = _db.ReceivedEmails.AsQueryable();

            if (includeAttachments)
            {
                qry = qry.Include(e => e.Attachments);
            }

            return qry.FirstOrDefault(e => e.Id == emailId)?.Adapt<EmailDto>();
        }

        public InboxIndexData GetData(Guid ownerId, bool unhandledSupplierOffersOnly, int page, int perPage, string sortField, bool sortAsc, string search)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = nameof(ReceivedEmail.MessageDate);
            }

            var qry = from item in _db.ReceivedEmails
                      where item.OwnerId == ownerId
                      select item;

            if (unhandledSupplierOffersOnly)
            {
                qry = qry.Where(q => !q.IsProcessed);
            }

            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc ? "" : " DESC")}");
            var query = orderedResults.Skip((page - 1) * perPage).Take(perPage);
            var qryResult = (from item in query
                             select new
                             {
                                 item.MessageDate,
                                 item.Id,
                                 item.Subject,
                                 item.FromEmail,
                                 item.OwnerId,
                                 item.Body,
                                 attachments = item.Attachments.Select(a => new
                                 {
                                     a.FileName,
                                     a.Id
                                 })
                             }).ToList();

            var result = (from item in qryResult
                          //let supplierId = _supplierService.GetSupplierByEmail(item.OwnerId, item.FromEmail)
                          //let supplier = _supplierService.GetById(supplierId, true)
                          select new InboxIndexDataItem
                          {
                              SupplierName = "",//supplier?.Name,
                              Id = item.Id,
                              MessageDate = item.MessageDate,
                              Subject = item.Subject,
                              Body = item.Body,
                              Attachments = item.attachments.Select(a => new InboxIndexAttachment
                              {
                                  FileName = a.FileName,
                                  Id = a.Id
                              }).ToList()
                          }).ToList();

            return new InboxIndexData
            {
                Total = total,
                Data = result
            };
        }

        public EmailAttachmentDto GetAttachment(Guid attachmentId) =>
            _db.EmailAttachments.FirstOrDefault(a => a.Id == attachmentId)?.Adapt<EmailAttachmentDto>();
    }
}
