using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Emails;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DigitalPurchasing.Services
{
    public class CompetitionListService : ICompetitionListService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICounterService _counterService;
        private readonly ISupplierOfferService _supplierOfferService;
        private readonly IRootService _rootService;
        private readonly IEmailService _emailService;
        private readonly LinkGenerator _linkGenerator;
        private readonly IQuotationRequestService _quotationRequestService;
        private readonly AppSettings _settings;
        private readonly IUserService _userService;

        public CompetitionListService(
            ApplicationDbContext db,
            ICounterService counterService,
            ISupplierOfferService supplierOfferService,
            IRootService rootService,
            IEmailService emailService,
            LinkGenerator linkGenerator,
            IQuotationRequestService quotationRequestService,
            IConfiguration configuration,
            IUserService userService)
        {
            _db = db;
            _counterService = counterService;
            _supplierOfferService = supplierOfferService;
            _rootService = rootService;
            _emailService = emailService;
            _linkGenerator = linkGenerator;
            _quotationRequestService = quotationRequestService;
            _userService = userService;
            _settings = configuration.GetSection(Consts.Settings.AppPath).Get<AppSettings>();
        }

        public async Task<int> CountByCompany(Guid companyId) => await _db.CompetitionLists.IgnoreQueryFilters().CountAsync(q => q.OwnerId == companyId);

        public CompetitionListIndexData GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Id";
            }

            var qry =  _db.CompetitionLists
                .Include(q => q.QuotationRequest)
                    .ThenInclude(q => q.PurchaseRequest)
                .AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var results = orderedResults
                .Skip((page-1)*perPage)
                .Take(perPage)
                .ProjectToType<CompetitionListIndexDataItem>()
                .ToList();

            var ids = results
                .Select(q => q.Id)
                .ToList();

            var clSuppliers = _db.SupplierOffers
                .Include(q => q.Supplier)
                .Where(q => ids.Contains(q.CompetitionListId))
                .Select(q => new
                {
                    CLId = q.CompetitionListId,
                    SupplierId = q.SupplierId,
                    SupplierName = q.Supplier.Name
                })
                .ToList();

            foreach (var resultItem in results)
            {
                var itemSuppliers = clSuppliers
                    .Where(q => q.CLId == resultItem.Id && q.SupplierId.HasValue)
                    .GroupBy(q => q.SupplierId)
                    .Select(q => q.FirstOrDefault())
                    .ToList();

                if (itemSuppliers.Any())
                {
                    resultItem.Suppliers = string.Join(", ",
                        itemSuppliers.Select(q => q.SupplierName).OrderBy(q => q));
                }
            }

            return new CompetitionListIndexData
            {
                Data = results,
                Total = total
            };
        }

        public async Task<Guid> GetIdByQR(Guid qrId, bool globalSearch)
        {
            var qry = _db.CompetitionLists.AsQueryable();
            if (globalSearch)
            {
                qry = qry.IgnoreQueryFilters();
            }

            var competitionList = await qry.FirstOrDefaultAsync(q => q.QuotationRequestId == qrId);
            if (competitionList != null)
            {
                return competitionList.Id;
            }

            return await CreateFromQR(qrId);
        }

        private async Task<Guid> CreateFromQR(Guid qrId)
        {
            var qr = await _db.QuotationRequests
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(q => q.Id == qrId);

            if (qr == null) return Guid.Empty;

            var entity = new CompetitionList
            {
                PublicId = _counterService.GetCLNextId(qr.OwnerId),
                QuotationRequestId = qr.Id,
                OwnerId = qr.OwnerId,
                IsClosed = false
            };
            var entry = await _db.CompetitionLists.AddAsync(entity);
            await _db.SaveChangesAsync();
            var clId = entry.Entity.Id;
            var rootId = await _rootService.GetIdByQR(qrId);
            await _rootService.AssignCL(qr.OwnerId, rootId, clId);
            return clId;
        }

        public CompetitionListVm GetById(Guid id, bool globalSearch = false)
        {
            var competitionListsQry = _db.CompetitionLists.AsNoTracking().AsQueryable();

            if (globalSearch)
            {
                competitionListsQry = competitionListsQry.IgnoreQueryFilters();
            }

            var competitionList = competitionListsQry.FirstOrDefault(q => q.Id == id);
            
            var vm = competitionList?.Adapt<CompetitionListVm>();

            if (vm != null)
            {
                var quotationRequestsQry = _db.QuotationRequests.AsNoTracking().AsQueryable();
                var purchaseRequestsQry = _db.PurchaseRequests
                    .Include(q => q.Customer)
                    .Include(q => q.Items)
                        .ThenInclude(q => q.Nomenclature)
                            .ThenInclude(q => q.BatchUom)
                    .AsNoTracking()
                    .AsQueryable();

                var supplierOffersQry = _db.SupplierOffers
                    .AsNoTracking()
                    .AsQueryable();

                if (globalSearch)
                {
                    quotationRequestsQry = quotationRequestsQry.IgnoreQueryFilters();
                    purchaseRequestsQry = purchaseRequestsQry.IgnoreQueryFilters();
                    supplierOffersQry = supplierOffersQry.IgnoreQueryFilters();
                }

                var quotationRequest = quotationRequestsQry.First(q => q.Id == competitionList.QuotationRequestId);
                var purchaseRequest = purchaseRequestsQry.First(q => q.Id == quotationRequest.PurchaseRequestId);

                vm.PurchaseRequest = purchaseRequest.Adapt<CompetitionListVm.PurchaseRequestVm>();
                vm.PurchaseRequest.Items = vm.PurchaseRequest.Items.OrderBy(q => q.Position).ToList();

                var supplierOffersIds = supplierOffersQry
                    .AsNoTracking()
                    .Where(q => q.CompetitionListId == id)
                    .Select(q => q.Id)
                    .ToList();
                
                // mappings for items - SupplierOfferItemMappings
                var supplierOffers = supplierOffersIds.Select(q
                    => _supplierOfferService.GetDetailsById(q, globalSearch)).ToList();

                vm.SupplierOffers = supplierOffers;

                foreach (var supplierOffer in vm.SupplierOffers)
                {
                    supplierOffer.Items = supplierOffer.Items.OrderBy(q => q.Position).ToList();

                    if (supplierOffer.Items.Count < vm.PurchaseRequest.Items.Count)
                    {
                        for (var i = 0; i < vm.PurchaseRequest.Items.Count; i++)
                        {
                            if (supplierOffer.Items.Count < i + 1 || vm.PurchaseRequest.Items[i].NomenclatureId !=
                                supplierOffer.Items[i].NomenclatureId)
                            {
                                supplierOffer.Items.Insert(i, null);
                            }
                        }
                    }
                }
            }

            return vm;
        }

        public DeleteResultVm Delete(Guid id)
        {
            var cl = _db.CompetitionLists.Find(id);
            if (cl == null) return DeleteResultVm.Success();

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var soIds = _db.SupplierOffers.Where(q => q.CompetitionListId == id).Select(q => q.Id).ToList();
                    foreach (var soId in soIds)
                    {
                        _supplierOfferService.Delete(soId);
                    }

                    _db.CompetitionLists.Remove(cl);
                    _db.SaveChanges();
                    transaction.Commit();
                    return new DeleteResultVm(true, null);
                }
                catch (Exception e)
                {
                    //todo: log e
                    transaction.Rollback();
                    return DeleteResultVm.Failure("Внутренняя ошибка. Обратитесь в службу поддержки");
                }
            }
        }

        public async Task SavePriceReductionEmail(
            Guid supplierOfferId,
            Guid supplierContactPersonId,
            Guid? userId,
            List<Guid> ids,
            PriceReductionSendingType sender)
        {
            var priceReductionEmail = new PriceReductionEmail
            {
                SupplierOfferId = supplierOfferId,
                ContactPersonId = supplierContactPersonId,
                UserId = userId,
                Data = JsonConvert.SerializeObject(ids),
                SendingType = sender
            };
            await _db.PriceReductionEmails.AddAsync(priceReductionEmail);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsPriceReductionEmailSent(Guid supplierOfferId)
            => await _db.PriceReductionEmails.AnyAsync(q => q.SupplierOfferId == supplierOfferId);

        public async Task<IEnumerable<PriceReductionEmailDto>> GetPriceReductionEmailsByCL(Guid competitionListId)
        {
            var emails = await _db.PriceReductionEmails.Include(q => q.SupplierOffer)
                .Where(q => q.SupplierOffer.CompetitionListId == competitionListId).ToListAsync();
            return emails.Adapt<List<PriceReductionEmailDto>>();
        }

        public async Task Close(Guid competitionListId)
        {
            var cl = await _db.CompetitionLists.FindAsync(competitionListId);
            cl.IsClosed = true;
            await _db.SaveChangesAsync();
            await SendCLClosedEmail(cl.Id, cl.PublicId);
        }

        public async Task CloseExpired()
        {
            var now = DateTime.UtcNow;
            var cls = await _db.CompetitionLists
                .IgnoreQueryFilters()
                .Where(q => q.IsClosed.HasValue && q.IsClosed.Value == false && q.AutomaticCloseDate <= now)
                .ToListAsync();
            foreach (var cl in cls)
            {
                cl.IsClosed = true;
                await _db.SaveChangesAsync();
                await SendCLClosedEmail(cl.Id, cl.PublicId);
            }
        }
        
        public async Task SetAutomaticCloseInHours(Guid competitionListId, double hours)
        {
            var cl = await _db.CompetitionLists.FindAsync(competitionListId);
            cl.AutomaticCloseDate = DateTime.UtcNow.AddHours(hours);
            await _db.SaveChangesAsync();
        }

        public async Task<PriceReductionDataVm> PriceReductionData(Guid competitionListId, Guid userId)
        {
            var cl = GetById(competitionListId, true);

            var offers = cl.GroupBySupplier()
                .Where(q => q.Key.SupplierId.HasValue)
                .Select(q => q.Value.Last())
                .ToList();

            var emails = (await GetPriceReductionEmailsByCL(competitionListId)).ToList();

            var user = await _userService.GetById(userId);

            var vm = new PriceReductionDataVm
            {
                Suppliers = offers.OrderBy(q => q.CreatedOn).Select(q => new PriceReductionDataVm.Supplier
                {
                    Id = q.Id,
                    Name = q.SupplierName,
                    CreatedOn = q.CreatedOn,
                    IsChecked = true
                }).ToList(),

                Items = cl.PurchaseRequest.Items.Select(pri =>
                {
                    var itemEmails = emails.Where(q => q.Data.Contains(pri.Id)).ToList();
                    
                    return new PriceReductionDataVm.Item
                    {
                        Id = pri.Id,
                        Position = pri.Position,
                        Discount = user.PRDiscountPercentage,
                        MinPrice = cl.GetMinimalOfferPrice(pri.Id),
                        MinPriceSOIds = cl.GetSOIdsWMinimalPrice(pri.Id),
                        Suppliers = offers.OrderBy(q => q.CreatedOn).Select(so =>
                        {
                            return new PriceReductionDataVm.ItemSupplier
                            {
                                Id = so.Id,
                                IsChecked = true,
                                IsEnabled = so.Items.Any(soi
                                    => soi.Request.ItemId == pri.Id && soi.Offer.Price > 0),
                                IsSent = itemEmails.Any(q => q.SupplierOfferId == so.Id)
                            };
                        }).ToList(),
                        SentDate = itemEmails.Any()
                            ? itemEmails.Max(q => q.CreatedOn).ToRussianStandardTime()
                            : (DateTime?)null
                    };
                }).ToList()
            };

            return vm;
        }

        private async Task SendCLClosedEmail(Guid clId, int publicId)
        {
            var root = await _rootService.GetByCL(clId);
            var email = _quotationRequestService.RequestSentBy(root.QuotationRequestId.Value);
            var url = _linkGenerator.GetPathByAction("Edit", "CompetitionList", new {id = clId});
            var fullUrl = $"{_settings.DefaultDomain}{url}";
            await _emailService.SendCLClosedEmail(email, fullUrl, publicId);
        }
    }
}
