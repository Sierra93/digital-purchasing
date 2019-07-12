using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class ReceivedEmailService : IReceivedEmailService
    {
        private readonly ApplicationDbContext _db;
        private readonly ISupplierService _supplierService;
        private readonly LinkGenerator _linkGenerator;

        public ReceivedEmailService(
            ApplicationDbContext db,
            ISupplierService supplierService,
            LinkGenerator linkGenerator)
        {
            _db = db;
            _supplierService = supplierService;
            _linkGenerator = linkGenerator;
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

        public void SetRoot(Guid emailId, Guid? rootId)
        {
            var email = _db.ReceivedEmails.Find(emailId);
            email.RootId = rootId;
            _db.SaveChanges();
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

            var qry = _db.ReceivedEmails
                .Include(q => q.Root.PurchaseRequest)
                .Include(q => q.Root.QuotationRequest)
                .Include(q => q.Root.CompetitionList)
                .Include(q => q.Attachments)
                .Where(q => q.OwnerId == ownerId);

            if (unhandledSupplierOffersOnly)
            {
                qry = qry.Where(q => !q.IsProcessed);
            }

            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc ? "" : " DESC")}");
            var query = orderedResults.Skip((page - 1) * perPage).Take(perPage);
            var qryResults = query.Select(item => new
            {
                item.MessageDate,
                item.Id,
                item.Subject,
                item.FromEmail,
                item.OwnerId,
                item.Body,
                item.IsProcessed,
                PRErp = item.Root != null && item.Root.PurchaseRequest != null ? item.Root.PurchaseRequest.ErpCode : null,
                CustomerName = item.Root != null && item.Root.PurchaseRequest != null && item.Root.PurchaseRequest.Customer != null ? item.Root.PurchaseRequest.Customer.Name : null,
                PRId = item.Root != null ? item.Root.PurchaseRequestId : null,
                QRId = item.Root != null ? item.Root.QuotationRequestId : null,
                CLId = item.Root != null ? item.Root.CompetitionListId : null,
                PRPublicId = item.Root != null && item.Root.PurchaseRequest != null ? item.Root.PurchaseRequest.PublicId : (int?)null,
                QRPublicId = item.Root != null && item.Root.QuotationRequest != null ? item.Root.QuotationRequest.PublicId : (int?)null,
                CLPublicId = item.Root != null && item.Root.CompetitionList != null ? item.Root.CompetitionList.PublicId : (int?)null,
                Attachments = item.Attachments.Select(a => new
                {
                    a.FileName,
                    a.Id
                })
            }).ToList();

            var data = new List<InboxIndexDataItem>();

            foreach (var item in qryResults)
            {
                var supplierName = "";

                if (item.OwnerId.HasValue)
                {
                    supplierName = _supplierService.GetSupplierNameByEmail(item.OwnerId.Value, item.FromEmail);
                }

                var prUrl = item.PRId.HasValue
                    ? _linkGenerator.GetPathByAction("Edit", "PurchaseRequest", new {id = item.PRId.Value})
                    : null;
                var qrUrl = item.QRId.HasValue
                    ? _linkGenerator.GetPathByAction("View", "QuotationRequest", new {id = item.QRId.Value})
                    : null;
                var clUrl = item.CLId.HasValue
                    ? _linkGenerator.GetPathByAction("Edit", "CompetitionList", new {id = item.CLId.Value})
                    : null;

                data.Add(new InboxIndexDataItem
                {
                    SupplierName = supplierName,
                    Id = item.Id,
                    MessageDate = item.MessageDate,
                    Subject = item.Subject,
                    Body = item.Body,
                    FromEmail = item.FromEmail,
                    IsProcessed = item.IsProcessed,
                    Attachments = item.Attachments.Select(a => new InboxIndexAttachment
                    {
                        FileName = a.FileName,
                        Id = a.Id
                    }).ToList(),
                    PRLink = prUrl != null ? $"<a href=\"{prUrl}\">{item.PRPublicId}</a>" : null,
                    QRLink = qrUrl != null ? $"<a href=\"{qrUrl}\">{item.QRPublicId}</a>" : null,
                    CLLink = clUrl != null ? $"<a href=\"{clUrl}\">{item.CLPublicId}</a>" : null,
                    CustomerName = item.CustomerName,
                    PRErp = item.PRErp
                });
            }

            return new InboxIndexData
            {
                Total = total,
                Data = data
            };
        }

        public EmailAttachmentDto GetAttachment(Guid attachmentId) =>
            _db.EmailAttachments.FirstOrDefault(a => a.Id == attachmentId)?.Adapt<EmailAttachmentDto>();
    }
}
