using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class NomenclatureCategoryService : INomenclatureCategoryService
    {
        private readonly ApplicationDbContext _db;

        public NomenclatureCategoryService(ApplicationDbContext dbContext) => _db = dbContext;

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

            foreach (var categoryResult in result)
            {
                categoryResult.FullName = FullCategoryName(categoryResult.Id);
            }

            return new NomenclatureCategoryDataResult
            {
                Total = total,
                Data = result
            };
        }

        public string FullCategoryName(Guid categoryId)
        {
            var category = _db.NomenclatureCategories.Find(categoryId);
            var name = category.Name;

            while (category.ParentId.HasValue)
            {
                category = _db.NomenclatureCategories.Find(category.ParentId);
                name = category.Name + " > " + name;
            }

            return name;
        }

        public NomenclatureCategoryResult CreateOrUpdate(string name, Guid? parentId)
        {
            var oldEntity = _db.NomenclatureCategories.FirstOrDefault(
                q => q.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && q.ParentId == parentId);

            if (oldEntity != null)
            {
                return oldEntity.Adapt<NomenclatureCategoryResult>();
            }

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
}
