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

        public NomenclatureDataResult GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry = _db.Nomenclatures.Where(q => !q.IsDeleted);
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<NomenclatureDataResultItem>().ToList();

            foreach (var nomenclatureResult in result)
            {
                nomenclatureResult.CategoryFullName = _categoryService.FullCategoryName(nomenclatureResult.CategoryId);
            }

            return new NomenclatureDataResult
            {
                Total = total,
                Data = result
            };
        }

        public NomenclatureResult CreateOrUpdate(NomenclatureResult vm)
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
                return oldEntity.Adapt<NomenclatureResult>();
            }

            var entity = vm.Adapt<Nomenclature>();
            var entry = _db.Nomenclatures.Add(entity);
            _db.SaveChanges();
            var result = entry.Entity.Adapt<NomenclatureResult>();
            return result;
        }

        public NomenclatureResult GetById(Guid id)
        {
            var entity = _db.Nomenclatures.Find(id);
            var result = entity.Adapt<NomenclatureResult>();
            return result;
        }

        public bool Update(NomenclatureResult model)
        {
            var entity = _db.Nomenclatures.Find(model.Id);
            if (entity == null) return false;

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

        public NomenclatureAutocompleteResult Autocomplete(string q, bool alts = false, string customer = null)
        {
            var result =  new NomenclatureAutocompleteResult
            {
                Items = new List<NomenclatureAutocompleteResult.AutocompleteResultItem>()
            };

            if (string.IsNullOrEmpty(q)) return result;

            q = q.Trim();

            var resultQry = _db.Nomenclatures
                .AsNoTracking()
                .Include(w => w.BatchUom)
                .Where(w => !w.IsDeleted &&
                    (w.Name.Contains(q, StringComparison.InvariantCultureIgnoreCase)
                        || w.NameEng.Contains(q, StringComparison.InvariantCultureIgnoreCase)
                        || w.Code.Contains(q, StringComparison.InvariantCultureIgnoreCase)));

            var mainResults = resultQry.ToList();
            
            if (alts)
            {
                var altNomIds = _db.NomenclatureAlternatives
                    .Where(w => w.Name.Contains(q, StringComparison.InvariantCultureIgnoreCase))
                    .Select(w => w.NomenclatureId).ToList();

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
            var entity = _db.Nomenclatures.FirstOrDefault(q => q.Id == id && !q.IsDeleted);
            if (entity == null) return;
            entity.IsDeleted = true;
            _db.SaveChanges();
        }

        public void AddAlternative(Guid nomenclatureId, Guid prItemId)
        {
            var pr =_db.PurchaseRequestItems.Include(q => q.PurchaseRequest).First(q => q.Id == prItemId);
            AddAlternative(nomenclatureId, pr.RawName, pr.PurchaseRequest.CustomerName);
        }

        public void AddAlternative(Guid nomenclatureId, string name, string customerName)
        {
            name = name.Trim();

            var entity = _db.Nomenclatures.Find(nomenclatureId);
            if (entity == null) return;

            if (!name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase) && !name.Equals(entity.NameEng, StringComparison.InvariantCultureIgnoreCase))
            {
                if (!_db.NomenclatureAlternatives.Any(q => q.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    _db.NomenclatureAlternatives.Add(new NomenclatureAlternative
                    {
                        NomenclatureId = nomenclatureId,
                        Name = name,
                        CustomerName = customerName
                    });
                    _db.SaveChanges();
                }
            }
        }
    }
}
