using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;

namespace DigitalPurchasing.Services
{
    public interface INomenclatureService
    {
        NomenclatureDataResult GetData(int page, int perPage, string sortField, bool sortAsc);
        NomenclatureResult Create(NomenclatureResult model);
        NomenclatureResult GetById(Guid id);
        bool Update(NomenclatureResult model);
    }

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
    }

    public class NomenclatureResult
    {
        public Guid Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }
        public string NameEng { get; set; }

        public Guid BatchUomId { get; set; }
        public string BatchUomName { get; set; }

        public Guid MassUomId { get; set; }
        public string MassUomName { get; set; }
        public decimal MassUomValue { get; set; }
       
        public Guid ResourceUomId { get; set; }
        public string ResourceUomName { get; set; }
        public decimal ResourceUomValue { get; set; }

        public Guid ResourceBatchUomId { get; set; }
        public string ResourceBatchUomName { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class NomenclatureDataResult : BaseDataResult<NomenclatureResult>
    {
    }
}
