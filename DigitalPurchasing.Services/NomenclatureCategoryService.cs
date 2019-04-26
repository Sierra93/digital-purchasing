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
            qry = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}")
                .Skip((page - 1) * perPage)
                .Take(perPage);
            var result = qry.ProjectToType<NomenclatureCategoryIndexDataItem>().ToList();

            var supplier2nomCategories = (from ncat in qry
                                          join n in _db.Nomenclatures on ncat.Id equals n.CategoryId
                                          join na in _db.NomenclatureAlternatives on n.Id equals na.NomenclatureId
                                          join nal in _db.NomenclatureAlternativeLinks on na.Id equals nal.AlternativeId
                                          join s in _db.Suppliers on nal.SupplierId equals s.Id                                          
                                          where !n.IsDeleted
                                          group new
                                          {
                                              supplierId = s.Id,
                                              supplierName = s.Name,
                                              categoryId = n.CategoryId
                                          } by new { s.Id, n.CategoryId } into g
                                          select g.FirstOrDefault()).ToList();

            var suppliersWithDefaultCategory = (from ncat in qry
                                                join s in _db.Suppliers on ncat.Id equals s.CategoryId
                                                where !ncat.IsDeleted
                                                group new
                                                {
                                                    supplierId = s.Id,
                                                    supplierName = s.Name,
                                                    categoryId = ncat.Id
                                                } by new { s.Id, catId = ncat.Id } into g
                                                select g.FirstOrDefault()).ToList();

            supplier2nomCategories = (from item in supplier2nomCategories.Union(suppliersWithDefaultCategory)
                                      group new
                                      {
                                          item.supplierId,
                                          item.supplierName,
                                          item.categoryId
                                      } by new { item.supplierId, item.categoryId } into g
                                      select g.FirstOrDefault()).ToList();

            foreach (var categoryResult in result)
            {
                categoryResult.Suppliers = supplier2nomCategories
                    .Where(_ => _.categoryId == categoryResult.Id)
                    .OrderBy(_ => _.supplierName)
                    .Select(_ => new NomenclatureCategoryIndexDataItem.SupplierInfo()
                    {
                        Id = _.supplierId,
                        Name = _.supplierName
                    })                    
                    .ToList();

                categoryResult.CategoriyHiearchy = GetCategoryHierarchy(categoryResult.Id).ToList();
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
            var hierarchy = GetCategoryHierarchy(categoryId).ToList();
            return string.Join(" > ", hierarchy.Select(_ => _.Name));
        }

        public NomenclatureCategoryBasicInfo GetTopParentCategory(Guid categoryId)
        {
            var hierarchy = GetCategoryHierarchy(categoryId).ToList();
            return hierarchy.FirstOrDefault()?.Adapt<NomenclatureCategoryBasicInfo>();
        }

        private IEnumerable<NomenclatureCategoryBasicInfo> GetCategoryHierarchy(Guid categoryId)
        {
            var category = _db.NomenclatureCategories.Find(categoryId);

            if (category.ParentId.HasValue)
            {
                foreach (var parent in GetCategoryHierarchy(category.ParentId.Value))
                {
                    yield return parent;
                }
            }

            yield return category.Adapt<NomenclatureCategoryBasicInfo>();
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
}
