using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IUomService
    {
        UomDataResponse GetData(int page, int perPage, string sortField, bool sortAsc);
        UomResult CreateOrUpdate(string name);
        IEnumerable<UomResult> GetAll();
        UomConversionRateResponse GetConversionRate(Guid fromUomId, Guid nomenclatureId);
        UomAutocompleteResponse Autocomplete(string s);
        BaseResult<UomAutocompleteResponse.AutocompleteItem> AutocompleteSingle(Guid id);
        void SaveConversionRate(Guid fromUomId, Guid toUomId, Guid nomenclatureId, decimal factorC, decimal factorN);
    }

    public class UomResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSystem { get; set; }
    }

    public class UomDataResponse : BaseDataResponse<UomResult>
    {
    }

    public class UomConversionRateResponse
    {
        public decimal NomenclatureFactor { get; set; }
        public decimal CommonFactor { get; set; }
    }

    public class UomAutocompleteResponse
    {
        public class AutocompleteItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public List<AutocompleteItem> Items = new List<AutocompleteItem>();
    }
}
