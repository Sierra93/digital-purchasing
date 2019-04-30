using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface INomenclatureCategoryService
    {
        NomenclatureCategoryIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        NomenclatureCategoryVm CreateOrUpdate(string name, Guid? parentId);
        IEnumerable<NomenclatureCategoryVm> GetAll(bool includeDeleted = false);
        NomenclatureCategoryVm GetById(Guid id);
        string FullCategoryName(Guid categoryId);
        void Delete(Guid id);
        NomenclatureCategoryVm Update(Guid id, string name, Guid? parentId);
        NomenclatureCategoryBasicInfo GetTopParentCategory(Guid categoryId);
    }

    public class NomenclatureCategoryVm : NomenclatureCategoryBasicInfo
    {
        public string FullName { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class NomenclatureCategoryIndexDataItem : NomenclatureCategoryBasicInfo
    {
        public class SupplierInfo
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public List<SupplierInfo> Suppliers = new List<SupplierInfo>();
        public List<NomenclatureCategoryBasicInfo> CategoriyHiearchy = new List<NomenclatureCategoryBasicInfo>();
    }

    public class NomenclatureCategoryIndexData : BaseDataResponse<NomenclatureCategoryIndexDataItem>
    {
    }

    public class NomenclatureCategoryBasicInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
    }
}
