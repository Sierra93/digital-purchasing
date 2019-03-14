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

        public CompetitionListService(
            ApplicationDbContext db,
            ICounterService counterService,
            ISupplierOfferService supplierOfferService)
        {
            _db = db;
            _counterService = counterService;
            _supplierOfferService = supplierOfferService;
        }

        public async Task<int> CountByCompany(Guid companyId) => await _db.CompetitionLists.IgnoreQueryFilters().CountAsync(q => q.OwnerId == companyId);

        public CompetitionListIndexData GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Id";
            }

            var qry =  _db.CompetitionLists.AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults
                .Skip((page-1)*perPage)
                .Take(perPage)
                .ProjectToType<CompetitionListIndexDataItem>()
                .ToList();

            return new CompetitionListIndexData
            {
                Data = result,
                Total = total
            };
        }

        public Guid GetIdByQR(Guid qrId, bool globalSearch)
        {
            var qry = _db.CompetitionLists.AsQueryable();
            if (globalSearch)
            {
                qry = qry.IgnoreQueryFilters();
            }

            var competitionList = qry.FirstOrDefault(q => q.QuotationRequestId == qrId);
            if (competitionList != null)
            {
                return competitionList.Id;
            }

            return CreateFromQR(qrId);
        }

        private Guid CreateFromQR(Guid qrId)
        {
            var qr = _db.QuotationRequests.IgnoreQueryFilters().First(q => q.Id == qrId);
            var entity = new CompetitionList
            {
                PublicId = _counterService.GetCLNextId(qr.OwnerId),
                QuotationRequestId = qr.Id,
                OwnerId = qr.OwnerId
            };
            var entry = _db.CompetitionLists.Add(entity);
            _db.SaveChanges();
            return entry.Entity.Id;
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
                var purchaseRequest = _db.PurchaseRequests.Include(q => q.Items).First(q => q.Id == quotationRequest.PurchaseRequestId);

                var nomIds = purchaseRequest.Items.Where(q => q.NomenclatureId.HasValue).Select(q => q.NomenclatureId.Value).ToList();

                vm.PurchaseRequest = purchaseRequest.Adapt<CompetitionListVm.PurchaseRequestVm>();
                vm.PurchaseRequest.Items = vm.PurchaseRequest.Items.OrderBy(q => q.NomenclatureId).ToList();
                var idx = 0;
                foreach (var requestItem in vm.PurchaseRequest.Items)
                {
                    requestItem.Position = ++idx;
                }

                var supplierOffers = _db.SupplierOffers.AsNoTracking().Include(q => q.Currency).Where(q => q.CompetitionListId == id).ToList();
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
