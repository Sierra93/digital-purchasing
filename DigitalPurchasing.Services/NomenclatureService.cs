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

namespace DigitalPurchasing.Services
{
    public class NomenclatureService : INomenclatureService
    {
        private readonly ApplicationDbContext _db;

        public NomenclatureService(ApplicationDbContext dbContext) => _db = dbContext;

        public NomenclatureDataResult GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry = _db.Nomenclatures.AsQueryable();
            var total = _db.Nomenclatures.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<NomenclatureResult>().ToList();

            return new NomenclatureDataResult
            {
                Total = total,
                Data = result
            };
        }

        public NomenclatureResult Create(NomenclatureResult model)
        {
            var entry = _db.Nomenclatures.Add(model.Adapt<Nomenclature>());
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

        public NomenclatureAutocompleteResult Autocomplete(string q)
        {
            var resultQry = _db.Nomenclatures.AsNoTracking().Include(w => w.BatchUom).Where(w =>
                w.Name.Contains(q, StringComparison.InvariantCultureIgnoreCase)
                || w.NameEng.Contains(q, StringComparison.InvariantCultureIgnoreCase)
                || w.Code.Contains(q, StringComparison.InvariantCultureIgnoreCase));

            var result = resultQry.ToList();
            return new NomenclatureAutocompleteResult
            {
                Items = result.Adapt<List<NomenclatureAutocompleteResult.AutocompleteResultItem>>()
            };
        }

        public BaseResult<NomenclatureAutocompleteResult.AutocompleteResultItem> AutocompleteSingle(Guid id)
        {
            var entity = _db.Nomenclatures.AsNoTracking().Include(w => w.BatchUom).FirstOrDefault(q => q.Id == id);
            var data = entity?.Adapt<NomenclatureAutocompleteResult.AutocompleteResultItem>();
            return new BaseResult<NomenclatureAutocompleteResult.AutocompleteResultItem>(data);
        }
    }
}
