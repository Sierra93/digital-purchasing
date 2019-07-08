using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class CompetitionListService : ICompetitionListService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICounterService _counterService;
        private readonly ISupplierOfferService _supplierOfferService;
        private readonly IRootService _rootService;

        public CompetitionListService(
            ApplicationDbContext db,
            ICounterService counterService,
            ISupplierOfferService supplierOfferService,
            IRootService rootService)
        {
            _db = db;
            _counterService = counterService;
            _supplierOfferService = supplierOfferService;
            _rootService = rootService;
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
                    SupplierName = q.Supplier.Name
                })
                .ToList();

            foreach (var resultItem in results)
            {
                var itemSuppliers = clSuppliers.Where(q => q.CLId == resultItem.Id).ToList();
                if (itemSuppliers.Any())
                {
                    resultItem.Suppliers = string.Join(", ", itemSuppliers.Select(q => q.SupplierName));
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
                OwnerId = qr.OwnerId
            };
            var entry = await _db.CompetitionLists.AddAsync(entity);
            await _db.SaveChangesAsync();
            var clId = entry.Entity.Id;
            var rootId = await _rootService.GetIdByQR(qrId);
            await _rootService.AssignCL(qr.OwnerId, rootId, clId);
            return clId;
        }

        public CompetitionListVm GetById(Guid id)
        {
            var competitionList = _db.CompetitionLists
                .Include(q => q.SupplierOffers)
                .FirstOrDefault(q => q.Id == id);
            
            var vm = competitionList?.Adapt<CompetitionListVm>();

            if (vm != null)
            {
                var quotationRequest = _db.QuotationRequests.Find(competitionList.QuotationRequestId);
                var purchaseRequest = _db.PurchaseRequests
                    .Include(q => q.Customer)
                    .Include(q => q.Items)
                        .ThenInclude(q => q.Nomenclature)
                            .ThenInclude(q => q.BatchUom)
                    .First(q => q.Id == quotationRequest.PurchaseRequestId);

                var nomIds = purchaseRequest.Items.Where(q => q.NomenclatureId.HasValue).Select(q => q.NomenclatureId.Value).ToList();

                vm.PurchaseRequest = purchaseRequest.Adapt<CompetitionListVm.PurchaseRequestVm>();
                vm.PurchaseRequest.Items = vm.PurchaseRequest.Items.OrderBy(q => q.NomenclatureId).ToList();

                var supplierOffers = _db.SupplierOffers
                    .AsNoTracking()
                    .Include(q => q.Supplier)
                    .Include(q => q.Currency)
                    .Where(q => q.CompetitionListId == id)
                    .ToList();

                foreach (var supplierOffer in supplierOffers)
                {
                    supplierOffer.Items = _db.SupplierOfferItems.Include(q => q.RawUom)
                        .Where(q => q.NomenclatureId.HasValue && nomIds.Contains(q.NomenclatureId.Value) &&
                                    q.RawUomId.HasValue &&
                                    q.SupplierOfferId == supplierOffer.Id)
                        .ToList();
                }
                // mappings for items - SupplierOfferItemMappings
                vm.SupplierOffers = supplierOffers.Adapt<IEnumerable<CompetitionListVm.SupplierOfferVm>>().OrderBy(q => q.CreatedOn).ToList();

                foreach (var supplierOffer in vm.SupplierOffers)
                {
                    supplierOffer.Items = supplierOffer.Items.OrderBy(q => q.NomenclatureId).ToList();

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
    }
}
