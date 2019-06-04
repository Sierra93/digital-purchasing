using System;
using System.Collections.Generic;
using DigitalPurchasing.Core.Enums;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface INomenclatureService
    {
        NomenclatureIndexData GetData(int page, int perPage, string sortField, bool sortAsc, string search);
        NomenclatureDetailsData GetDetailsData(Guid nomId, int page, int perPage, string sortField, bool sortAsc, string sortBySearch);
        NomenclatureVm CreateOrUpdate(NomenclatureVm model);
        void CreateOrUpdate(List<NomenclatureVm> models, Guid ownerId);
        NomenclatureVm GetById(Guid id, bool globalSearch = false);
        IEnumerable<NomenclatureVm> GetByNames(params string[] nomenclatureNames);
        NomenclatureAutocompleteResult Autocomplete(AutocompleteOptions options);
        BaseResult<NomenclatureAutocompleteResult.AutocompleteResultItem> AutocompleteSingle(Guid id);
        void Delete(Guid id);
        NomenclatureWholeData GetWholeNomenclature();
        NomenclatureVm FindBestFuzzyMatch(Guid ownerId, string nomName);
    }

    public class NomenclatureWholeData
    {
        public IDictionary<NomenclatureIndexDataItem, NomenclatureDetailsData> Nomenclatures { get; set; } =
            new Dictionary<NomenclatureIndexDataItem, NomenclatureDetailsData>();
    }

    public class NomenclatureDetailsDataItem
    {
        public Guid Id { get; set; }

        public int ClientType { get; set; }
        public string ClientName { get; set; }
        public int? ClientPublicId { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

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

        public string PackUomName { get; set; }
        public decimal? PackUomValue { get; set; }
    }

    public class NomenclatureDetailsData : BaseDataResponse<NomenclatureDetailsDataItem>
    {
    }

    public class NomenclatureIndexDataItem
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

        public Guid? PackUomId { get; set; }
        public string PackUomName { get; set; }
        public decimal PackUomValue { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryFullName { get; set; }
        public bool HasAlternativeWithRequiredName { get; set; }
    }

    //todo: rename to NomenclatureDto
    public class NomenclatureVm
    {
        public Guid OwnerId { get; set; }

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

        public Guid? PackUomId { get; set; }
        public string PackUomName { get; set; }
        public decimal PackUomValue { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryFullName { get; set; }
    }

    public class NomenclatureIndexData : BaseDataResponse<NomenclatureIndexDataItem>
    {
    }

    public class AutocompleteBaseOptions
    {
        public string Query { get; set; }
    }

    public class AutocompleteOptions : AutocompleteBaseOptions
    {
        public Guid ClientId { get; set; }
        public ClientType ClientType { get; set; }
        public bool SearchInAlts { get; set; } = false;
        public Guid OwnerId { get; set; }
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
            public Guid BatchUomId { get; set; }
            public bool IsFullMatch { get; set; }
        }

        public List<AutocompleteResultItem> Items { get; set; } = new List<AutocompleteResultItem>();
    }
}
