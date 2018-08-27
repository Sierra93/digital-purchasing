using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public interface INomenclatureCategoryService
    {
        NomenclatureCategoryDataResult GetData(int page, int perPage, string sortField, bool sortAsc);
        NomenclatureCategoryResult CreateCategory(string name, Guid? parentId);
        IEnumerable<NomenclatureCategoryResult> GetAll();
    }

    public class NomenclatureCategoryService : INomenclatureCategoryService
    {
        private readonly ApplicationDbContext _db;

        public NomenclatureCategoryService(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }

        public NomenclatureCategoryDataResult GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry =  _db.NomenclatureCategories.Include(q => q.Parent).AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<NomenclatureCategoryResult>().ToList();
            return new NomenclatureCategoryDataResult
            {
                Total = total,
                Data = result
            };
        }

        public NomenclatureCategoryResult CreateCategory(string name, Guid? parentId)
        {
            var entry = _db.NomenclatureCategories.Add(new NomenclatureCategory
            {
                Name = name,
                ParentId = parentId
            });

            _db.SaveChanges();
            _db.Entry(entry.Entity).Reference(b => b.Owner).Load();

            return entry.Entity.Adapt<NomenclatureCategoryResult>();
        }

        public IEnumerable<NomenclatureCategoryResult> GetAll()
        {
            var result = _db.NomenclatureCategories.Include(q => q.Parent).ProjectToType<NomenclatureCategoryResult>().ToList();
            return result;
        }
    }

    public class MapsterRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<NomenclatureCategory, NomenclatureCategoryResult>().Map(d => d.ParentName, s => s.Parent != null ? s.Parent.Name : null);
        }
    }

    public class NomenclatureCategoryResult
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? ParentId { get; set; }

        public string ParentName { get; set; }
    }

    public class NomenclatureCategoryDataResult : BaseDataResponse<NomenclatureCategoryResult>
    {
    }
}
