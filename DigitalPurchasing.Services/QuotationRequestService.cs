using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Emails;
using DigitalPurchasing.ExcelReader;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class QuotationRequestService : IQuotationRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICounterService _counterService;
        private readonly IPurchaseRequestService _purchaseRequestService;
        private readonly IDeliveryService _deliveryService;
        private readonly ISupplierService _supplierService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IRootService _rootService;

        public QuotationRequestService(
            ApplicationDbContext db,
            ICounterService counterService,
            IPurchaseRequestService purchaseRequestService,
            IDeliveryService deliveryService,
            ISupplierService supplierService,
            IEmailService emailService,
            IUserService userService,
            IRootService rootService)
        {
            _db = db;
            _counterService = counterService;
            _purchaseRequestService = purchaseRequestService;
            _deliveryService = deliveryService;
            _supplierService = supplierService;
            _emailService = emailService;
            _userService = userService;
            _rootService = rootService;
        }

        public async Task<int> CountByCompany(Guid companyId) => await _db.QuotationRequests.IgnoreQueryFilters().CountAsync(q => q.OwnerId == companyId);

        public QuotationRequestIndexData GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Id";
            }

            var qry =  _db.QuotationRequests.AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults
                .Skip((page-1)*perPage)
                .Take(perPage)
                .ProjectToType<QuotationRequestIndexDataItem>()
                .ToList();

            return new QuotationRequestIndexData
            {
                Data = result,
                Total = total
            };
        }

        private async Task<Guid> CreateQuotationRequest(Guid purchaseRequestId)
        {
            var publicId = _counterService.GetQRNextId();
            var entry = _db.QuotationRequests.Add(
                new QuotationRequest
                {
                    PublicId = publicId,
                    PurchaseRequestId = purchaseRequestId
                });
            _db.SaveChanges();
            var qrId = entry.Entity.Id;
            var ownerId = entry.Entity.OwnerId;

            // copy delivery to QR
            var prDelivery = _deliveryService.GetByPrId(purchaseRequestId);
            prDelivery.Id = Guid.Empty;
            _deliveryService.CreateOrUpdate(prDelivery, null, qrId);

            var rootId = await _rootService.GetIdByPR(purchaseRequestId);

            await _rootService.AssignQR(ownerId, rootId, qrId);

            return qrId;
        }

        public async Task<Guid> GetQuotationRequestId(Guid purchaseRequestId)
        {
            var quotationRequest = _db.QuotationRequests.FirstOrDefault(q => q.PurchaseRequestId == purchaseRequestId);
            if (quotationRequest != null)
            {
                return quotationRequest.Id;
            }

            var data = _purchaseRequestService.MatchItemsData(purchaseRequestId);
            if (data == null) return Guid.Empty;

            if (data.Items.All(q => q.NomenclatureId.HasValue && q.RawUomMatchId.HasValue && (q.CommonFactor > 0 || q.NomenclatureFactor > 0 )))
            {
                return await CreateQuotationRequest(purchaseRequestId);
            }

            return Guid.Empty;;
        }

        public QuotationRequestVm GetById(Guid id, bool globalSearch = false)
        {
            var qry = _db.QuotationRequests
                .AsNoTracking()
                .AsQueryable();

            if (globalSearch)
            {
                qry = qry.IgnoreQueryFilters();
            }

            var quotationRequest = qry.FirstOrDefault(q => q.Id == id);
            var result = quotationRequest?.Adapt<QuotationRequestVm>();
            return result;
        }

        public QuotationRequestViewData GetViewData(Guid qrId)
        {
            var pr = (from item in _db.PurchaseRequests
                      join qr in _db.QuotationRequests on item.Id equals qr.PurchaseRequestId
                      where qr.Id == qrId
                      select new
                      {
                          id = item.Id,
                          companyName = item.CompanyName,
                          customerName = item.CustomerName
                      }).First();

            var data = _purchaseRequestService.MatchItemsData(pr.id);
            var uniqueCategoryIds = data.Items.Select(_ => _.NomenclatureCategoryId).Distinct();
            var sentRequests = GetSentRequests(qrId);
            var suppliersByCategories = _supplierService.GetByCategoryIds(uniqueCategoryIds.ToArray())
                .Where(s => !sentRequests.Any(sr => sr.SupplierId == s.Id));
            var result = new QuotationRequestViewData(pr.companyName, pr.customerName)
            {
                SentRequests = sentRequests,
                ApplicableSuppliers = suppliersByCategories.Select(_ => new QuotationRequestApplicableSupplier
                {
                    Id = _.Id,
                    Name = _.Name
                }).ToList()
            };

            foreach (var dataItem in data.Items)
            {
                var factor = dataItem.CommonFactor > 0 ? dataItem.CommonFactor : dataItem.NomenclatureFactor;
                var companyQty = dataItem.RawQty * factor;
                result.AddCompanyItem(dataItem.NomenclatureName, dataItem.NomenclatureCode, dataItem.NomenclatureUom, companyQty.ToString(CultureInfo.InvariantCulture));
                result.AddCustomerItem(dataItem.RawName, dataItem.RawCode, dataItem.RawUom, dataItem.RawQty.ToString(CultureInfo.InvariantCulture));
            }

            return result;
        }

        public DeleteResultVm Delete(Guid id)
        {
            var cl = _db.CompetitionLists.FirstOrDefault(q => q.QuotationRequestId == id);
            if (cl != null)
            {
                return DeleteResultVm.Failure($"Нельзя удалить заявку, сперва удалите конкурентный лист №{cl.PublicId}");
            }

            var qr = _db.QuotationRequests.Find(id);
            if (qr == null) return DeleteResultVm.Success();

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    if (qr.DeliveryId.HasValue)
                    {
                        _deliveryService.Delete(qr.DeliveryId.Value);
                    }
                    _db.QuotationRequests.Remove(qr);
                    _db.SaveChanges();
                    transaction.Commit();
                    return DeleteResultVm.Success();
                }
                catch (Exception e)
                {
                    //todo: log e
                    transaction.Rollback();
                    return DeleteResultVm.Failure("Внутренняя ошибка. Обратитесь в службу поддержки");
                }
            }
        }

        public async Task SendRequests(Guid userId, Guid quotationRequestId, IReadOnlyList<Guid> suppliers)
        {
            var qr = GetById(quotationRequestId);
            if (qr == null) return;

            // get supplier contacts
            var contacts = suppliers.Select(supplierId => _supplierService.GetContactPersonBySupplier(supplierId))
                .Where(q => q != null)
                .ToList();

            if (!contacts.Any()) return;

            // create excel and save
            var excelBytes = GenerateExcelForQR(quotationRequestId);
            var filename = $"RFQ_{qr.CreatedOn:yyyyMMdd}_{qr.PublicId}.xlsx";
            var filepath = Path.Combine(Path.GetTempPath(), filename);
            using (var fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(excelBytes, 0, excelBytes.Length);
            }
            
            // send emails for each contact
            var emailUid = QuotationRequestToUid(quotationRequestId);
            var userInfo = _userService.GetUserInfo(userId);
            foreach (var contact in contacts)
            {

                await _emailService.SendRFQEmail(qr, userInfo, contact, emailUid, filepath);
                await _db.QuotationRequestEmails.AddAsync(new QuotationRequestEmail
                {
                    RequestId = qr.Id,
                    ContactPersonId = contact.Id
                });
                await _db.SaveChangesAsync();
            }

            var rootId = await _rootService.GetIdByQR(quotationRequestId);
            await _rootService.SetStatus(rootId, RootStatus.QuotationRequestSent);
        }

        public byte[] GenerateExcelForQR(Guid quotationRequestId)
        {
            var qr = _db.QuotationRequests.Find(quotationRequestId);
            var prId = qr.PurchaseRequestId;
            var data = _purchaseRequestService.MatchItemsData(prId);;

            var items = new List<ExcelQr.DataItem>();
            foreach (var dataItem in data.Items)
            {
                var factor = dataItem.CommonFactor > 0 ? dataItem.CommonFactor : dataItem.NomenclatureFactor;
                var companyQty = dataItem.RawQty * factor;
                items.Add(new ExcelQr.DataItem
                {
                    Code = dataItem.NomenclatureCode,
                    Name = dataItem.NomenclatureName,
                    Uom = dataItem.NomenclatureUom,
                    Qty = companyQty
                });
            }

            var excel = new ExcelQr();
            var bytes = excel.Build(items);
            return bytes;
        }

        public List<SentRequest> GetSentRequests(Guid quotationRequestId)
        {
            var entities = _db.QuotationRequestEmails
                .Include(q => q.ContactPerson)
                .ThenInclude(q => q.Supplier)
                .Where(q => q.RequestId == quotationRequestId)
                .ToList();

            return entities.Select(q => new SentRequest
            {
                CreatedOn = q.CreatedOn,
                SupplierId = q.ContactPerson.SupplierId,
                SupplierName = q.ContactPerson.Supplier.Name,
                PersonFullName = q.ContactPerson.FullName,
                Email = q.ContactPerson.Email
            }).ToList();
        }

        public string QuotationRequestToUid(Guid quotationRequestId)
        {
            var qr = _db.QuotationRequests.Find(quotationRequestId);
            var md5Time = qr.CreatedOn.ToString("hh:mm:ss").ToMD5().Substring(0, 4).ToUpperInvariant();
            return $"RFQ-{qr.CreatedOn:yyMMdd}-{qr.PublicId}-{md5Time}";
        }

        public Guid? UidToQuotationRequest(string uid)
        {
            if (string.IsNullOrEmpty(uid) || !uid.StartsWith("RFQ-")) return Guid.Empty;

            var parts = uid.Split('-');
            var date = DateTime.ParseExact(parts[1], "yyMMdd", CultureInfo.InvariantCulture);
            var id = int.Parse(parts[2]);
            var strTime = parts[3];

            var qrs = _db.QuotationRequests.IgnoreQueryFilters().Where(q => q.PublicId == id && q.CreatedOn.Date == date).ToList();
            if (qrs.Count == 1)
            {
                return qrs[0].Id;
            }
            else
            {
                var qr = qrs.FirstOrDefault(q =>
                    q.CreatedOn.ToString("hh:mm:ss").ToMD5().Substring(0, 4).ToUpperInvariant() == strTime);
                return qr?.Id;
            }
        }
    }
}
