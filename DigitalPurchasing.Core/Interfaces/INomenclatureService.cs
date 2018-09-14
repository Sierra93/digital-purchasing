using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface INomenclatureService
    {
        NomenclatureDataResult GetData(int page, int perPage, string sortField, bool sortAsc);
        NomenclatureResult Create(NomenclatureResult model);
        NomenclatureResult GetById(Guid id);
        bool Update(NomenclatureResult model);
        NomenclatureAutocompleteResult Autocomplete(string q);
        BaseResult<NomenclatureAutocompleteResult.AutocompleteResultItem> AutocompleteSingle(Guid id);
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

    public class NomenclatureDataResult : BaseDataResponse<NomenclatureResult>
    {
    }

    public class NomenclatureAutocompleteResult
    {
        public class AutocompleteResultItem
        {
            public string Name { get; set; }
            public string NameEng { get; set; }
            public Guid Id { get; set; }
            public string Code { get; set; }
            public string BatchUomName { get; set; }

            public string FullName => string.IsNullOrEmpty(NameEng) ? $"[{Code}] {Name}" : $"[{Code}] {Name} / {NameEng}";
        }

        public List<AutocompleteResultItem> Items { get; set; } = new List<AutocompleteResultItem>();
    }
}
