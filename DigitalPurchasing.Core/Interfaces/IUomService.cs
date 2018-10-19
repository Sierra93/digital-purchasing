using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IUomService
    {
        UomIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        UomFactorData GetFactorData(Guid uomId, int page, int perPage, string sortField, bool sortAsc);
        UomResult CreateOrUpdate(string name);
        IEnumerable<UomResult> GetAll();
        UomConversionRateResponse GetConversionRate(Guid fromUomId, Guid nomenclatureId);
        UomAutocompleteResponse Autocomplete(string s);
        BaseResult<UomAutocompleteResponse.AutocompleteItem> AutocompleteSingle(Guid id);
        void SaveConversionRate(Guid fromUomId, Guid toUomId, Guid nomenclatureId, decimal factorC, decimal factorN);
        void Delete(Guid id);
        UomVm GetById(Guid id);
        UomVm Update(Guid id, string name);
    }

    public class UomResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class UomVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class UomFactorDataItem
    {
        public Guid Id { get; set; }
        public string Uom { get; set; }
        public decimal Factor { get; set; }
        public string Nomenclature { get; set; }
    }

    public class UomFactorData : BaseDataResponse<UomFactorDataItem>
    {
    }

    public class UomIndexDataItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class UomIndexData : BaseDataResponse<UomIndexDataItem>
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
