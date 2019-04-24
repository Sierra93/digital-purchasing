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

        public NomenclatureCategoryIndexData GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry =  _db.NomenclatureCategories.Where(q => !q.IsDeleted).Include(q => q.Parent).AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<NomenclatureCategoryIndexDataItem>().ToList();

            foreach (var categoryResult in result)
            {
                categoryResult.FullName = FullCategoryName(categoryResult.Id);
            }

            return new NomenclatureCategoryIndexData
            {
                Total = total,
                Data = result
            };
        }

        public NomenclatureCategoryVm GetById(Guid id) => _db.NomenclatureCategories.Find(id)?.Adapt<NomenclatureCategoryVm>();

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

        public NomenclatureCategoryBasicInfo GetParentCategory(Guid categoryId)
        {
            var category = _db.NomenclatureCategories.Find(categoryId);

            while (category.ParentId.HasValue)
            {
                category = _db.NomenclatureCategories.Find(category.ParentId);
            }

            return category.Adapt<NomenclatureCategoryBasicInfo>();
        }

        public void Delete(Guid id)
        {
            var entity = _db.NomenclatureCategories.Find(id);
            if (entity == null) return;
            entity.IsDeleted = true;
            _db.SaveChanges();
        }

        public NomenclatureCategoryVm Update(Guid id, string name, Guid? parentId)
        {
            var entity = _db.NomenclatureCategories.Find(id);
            if (entity == null) return null;
            entity.Name = name.Trim().Trim('>');
            entity.ParentId = parentId;
            _db.SaveChanges();
            _db.Entry(entity).Reference(q => q.Parent).Load();
            var result = entity.Adapt<NomenclatureCategoryVm>();
            result.FullName = FullCategoryName(result.Id);
            return result;
        }

        public NomenclatureCategoryVm CreateOrUpdate(string name, Guid? parentId)
        {
            var oldEntity = _db.NomenclatureCategories.FirstOrDefault(
                q => q.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && q.ParentId == parentId);

            if (oldEntity != null)
            {
                return oldEntity.Adapt<NomenclatureCategoryVm>();
            }

            var entry = _db.NomenclatureCategories.Add(new NomenclatureCategory
            {
                Name = name.Trim().Trim('>'),
                ParentId = parentId
            });

            _db.SaveChanges();
            _db.Entry(entry.Entity).Reference(b => b.Owner).Load();

            return entry.Entity.Adapt<NomenclatureCategoryVm>();
        }

        public IEnumerable<NomenclatureCategoryVm> GetAll()
        {
            var result = _db.NomenclatureCategories.Where(q => !q.IsDeleted).Include(q => q.Parent).ProjectToType<NomenclatureCategoryVm>().ToList();
            return result;
        }
    }

    public class NomenclatureCategoryMappings : IRegister
    {
        public void Register(TypeAdapterConfig config) => config.NewConfig<NomenclatureCategory, NomenclatureCategoryVm>().Map(d => d.ParentName, s => s.Parent != null ? s.Parent.Name : null);
    }
}
