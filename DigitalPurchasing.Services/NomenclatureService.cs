using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DigitalPurchasing.Services
{
    public class NomenclatureService : INomenclatureService
    {
        private readonly ApplicationDbContext _db;
        private readonly INomenclatureCategoryService _categoryService;

        public NomenclatureService(ApplicationDbContext dbContext, INomenclatureCategoryService categoryService)
        {
            _db = dbContext;
            _categoryService = categoryService;
        }

        public NomenclatureIndexData GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry = _db.Nomenclatures.Where(q => !q.IsDeleted);
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<NomenclatureIndexDataItem>().ToList();

            foreach (var nomenclatureResult in result)
            {
                nomenclatureResult.CategoryFullName = _categoryService.FullCategoryName(nomenclatureResult.CategoryId);
            }

            return new NomenclatureIndexData
            {
                Total = total,
                Data = result
            };
        }

        public NomenclatureDetailsData GetDetailsData(Guid nomId, int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry = _db.NomenclatureAlternatives.Where(q => q.NomenclatureId == nomId);
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<NomenclatureDetailsDataItem>().ToList();

            return new NomenclatureDetailsData
            {
                Total = total,
                Data = result
            };
        }

        public NomenclatureVm CreateOrUpdate(NomenclatureVm vm)
        {
            var oldEntity = _db.Nomenclatures.FirstOrDefault(q => q.Name.Equals(vm.Name, StringComparison.InvariantCultureIgnoreCase));
            if (oldEntity != null)
            {
                oldEntity.Code = vm.Code;
                oldEntity.BatchUomId = vm.BatchUomId;
                oldEntity.MassUomId = vm.MassUomId;
                oldEntity.ResourceUomId = vm.ResourceUomId;
                oldEntity.ResourceBatchUomId = vm.ResourceBatchUomId;
                oldEntity.ResourceUomValue = vm.ResourceUomValue;
                oldEntity.MassUomValue = vm.MassUomValue;
                _db.SaveChanges();
                return oldEntity.Adapt<NomenclatureVm>();
            }

            var entity = vm.Adapt<Nomenclature>();
            var entry = _db.Nomenclatures.Add(entity);
            _db.SaveChanges();
            var result = entry.Entity.Adapt<NomenclatureVm>();
            return result;
        }

        public NomenclatureVm GetById(Guid id)
        {
            var entity = _db.Nomenclatures.Find(id);
            var result = entity.Adapt<NomenclatureVm>();
            return result;
        }

        public bool Update(NomenclatureVm model)
        {
            var entity = _db.Nomenclatures.Find(model.Id);
            if (entity == null) return false;

            entity.CategoryId = model.CategoryId;
            entity.Code = model.Code;
            entity.Name = model.Name;
            entity.NameEng = model.NameEng;

            entity.ResourceUomId = model.ResourceUomId;
            entity.ResourceUomValue = model.ResourceUomValue;
            entity.ResourceBatchUomId = model.ResourceBatchUomId;

            entity.BatchUomId = model.BatchUomId;

            entity.MassUomId = model.MassUomId;

            entity.MassUomValue = model.MassUomValue;
            
            _db.SaveChanges();
            return true;
        }

        public NomenclatureAutocompleteResult Autocomplete(AutocompleteOptions options)
        {
            var result =  new NomenclatureAutocompleteResult
            {
                Items = new List<NomenclatureAutocompleteResult.AutocompleteResultItem>()
            };

            var q = options.Query.Trim();

            if (string.IsNullOrEmpty(q)) return result;

            var strComparison = StringComparison.InvariantCultureIgnoreCase;

            var resultQry = _db.Nomenclatures
                .AsNoTracking()
                .Include(w => w.BatchUom)
                .Where(w => !w.IsDeleted &&
                        (w.Name.Contains(q, strComparison) ||
                         w.NameEng.Contains(q, strComparison) ||
                         w.Code.Contains(q, strComparison)));

            var mainResults = resultQry.ToList();
            
            if (options.SearchInAlts)
            {
                var altNomIds = _db.NomenclatureAlternatives
                    .Where(w => w.Name.Contains(q, strComparison) &&
                                w.ClientName.Equals(options.ClientName, strComparison))
                    .Select(w => w.NomenclatureId)
                    .Distinct()
                    .ToList();

                if (altNomIds.Any())
                {
                    var mainNomIds = mainResults.Select(w => w.Id);
                    var altResults = _db.Nomenclatures.Where(w => altNomIds.Contains(w.Id) && !mainNomIds.Contains(w.Id) && !w.IsDeleted).ToList();
                    mainResults = mainResults.Union(altResults).ToList();
                }
            }

            result.Items.AddRange(mainResults.Adapt<List<NomenclatureAutocompleteResult.AutocompleteResultItem>>());

            return result;
        }

        public BaseResult<NomenclatureAutocompleteResult.AutocompleteResultItem> AutocompleteSingle(Guid id)
        {
            var entity = _db.Nomenclatures.AsNoTracking().Include(w => w.BatchUom).FirstOrDefault(q => q.Id == id);
            var data = entity?.Adapt<NomenclatureAutocompleteResult.AutocompleteResultItem>();
            return new BaseResult<NomenclatureAutocompleteResult.AutocompleteResultItem>(data);
        }

        public void Delete(Guid id)
        {
            var entity = _db.Nomenclatures.Find(id);
            if (entity == null) return;
            entity.IsDeleted = true;
            _db.SaveChanges();
        }

        public void AddAlternativeCustomer(Guid nomenclatureId, Guid prItemId)
        {
            var pr =_db.PurchaseRequestItems.Include(q => q.PurchaseRequest).First(q => q.Id == prItemId);
            AddAlternative(
                nomenclatureId,
                pr.PurchaseRequest.CustomerName,
                ClientType.Customer,
                pr.RawName,
                pr.RawCode,
                pr.RawUomMatchId);
        }

        public void AddAlternativeSupplier(Guid nomenclatureId, Guid soItemId)
        {
            var pr =_db.SupplierOfferItems.Include(q => q.SupplierOffer).First(q => q.Id == soItemId);
            AddAlternative(
                nomenclatureId,
                pr.SupplierOffer.SupplierName,
                ClientType.Supplier,
                pr.RawName,
                pr.RawCode,
                pr.RawUomId);
        }

        public NomenclatureAlternativeVm GetAlternativeById(Guid id)
        {
            var entity = _db.NomenclatureAlternatives.Find(id);
            var result = entity.Adapt<NomenclatureAlternativeVm>();
            return result;
        }

        public void UpdateAlternative(NomenclatureAlternativeVm model)
        {
            var entity = _db.NomenclatureAlternatives.Find(model.Id);

            entity.ClientName = model.ClientName;
            entity.ClientType = model.ClientType;
            entity.Name = model.Name;
            entity.Code = model.Code;

            entity.ResourceUomId = model.ResourceUomId;
            entity.ResourceUomValue = model.ResourceUomValue;

            entity.BatchUomId = model.BatchUomId;
            entity.ResourceBatchUomId = model.ResourceBatchUomId;

            entity.MassUomId = model.MassUomId;
            entity.MassUomValue = model.MassUomValue;
            
            _db.SaveChanges();
        }

        private void AddAlternative(Guid nomenclatureId, string clientName, ClientType clientType, string name, string code, Guid? uom)
        {
            clientName = clientName.Trim();
            name = name.Trim();
            code = code.Trim();

            var altName = _db.NomenclatureAlternatives
                .FirstOrDefault(q => q.NomenclatureId == nomenclatureId &&
                                     q.ClientName.Equals(clientName, StringComparison.InvariantCultureIgnoreCase) &&
                                     q.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (altName != null)
            {
                if (string.IsNullOrEmpty(altName.Code))
                    altName.Code = code;
                if (!altName.BatchUomId.HasValue)
                {
                    altName.BatchUomId = uom;
                }
            }
            else
            {
                _db.NomenclatureAlternatives.Add(new NomenclatureAlternative
                {
                    Name = name,
                    Code = code,
                    BatchUomId = uom,
                    ClientName = clientName,
                    ClientType = clientType,
                    NomenclatureId = nomenclatureId
                });
            }

            _db.SaveChanges();
        }
    }
}
