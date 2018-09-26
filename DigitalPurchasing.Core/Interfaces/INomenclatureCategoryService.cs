using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface INomenclatureCategoryService
    {
        NomenclatureCategoryDataResult GetData(int page, int perPage, string sortField, bool sortAsc);
        NomenclatureCategoryResult CreateOrUpdate(string name, Guid? parentId);
        IEnumerable<NomenclatureCategoryResult> GetAll();
        string FullCategoryName(Guid categoryId);
    }

    public class NomenclatureCategoryResult
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? ParentId { get; set; }

        public string ParentName { get; set; }

        public string FullName { get; set; }
    }

    public class NomenclatureCategoryDataResult : BaseDataResponse<NomenclatureCategoryResult>
    {
    }
}
