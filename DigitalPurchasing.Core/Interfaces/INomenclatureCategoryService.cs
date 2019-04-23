using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface INomenclatureCategoryService
    {
        NomenclatureCategoryIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        NomenclatureCategoryVm CreateOrUpdate(string name, Guid? parentId);
        IEnumerable<NomenclatureCategoryVm> GetAll();
        NomenclatureCategoryVm GetById(Guid id);
        string FullCategoryName(Guid categoryId);
        void Delete(Guid id);
        NomenclatureCategoryVm Update(Guid id, string name, Guid? parentId);
        NomenclatureCategoryBasicInfo GetParentCategory(Guid categoryId);
    }

    public class NomenclatureCategoryVm : NomenclatureCategoryBasicInfo
    {
        public Guid? ParentId { get; set; }

        public string ParentName { get; set; }

        public string FullName { get; set; }
    }

    public class NomenclatureCategoryIndexDataItem : NomenclatureCategoryVm
    {
    }

    public class NomenclatureCategoryIndexData : BaseDataResponse<NomenclatureCategoryIndexDataItem>
    {
    }

    public class NomenclatureCategoryBasicInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
