using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface INomenclatureService
    {
        NomenclatureIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        NomenclatureDetailsData GetDetailsData(Guid nomId, int page, int perPage, string sortField, bool sortAsc);
        NomenclatureVm CreateOrUpdate(NomenclatureVm model);
        NomenclatureVm GetById(Guid id);
        bool Update(NomenclatureVm model);
        NomenclatureAutocompleteResult Autocomplete(AutocompleteOptions options);
        BaseResult<NomenclatureAutocompleteResult.AutocompleteResultItem> AutocompleteSingle(Guid id);
        void Delete(Guid id);
        void AddNomenclatureForCustomer(Guid prItemId);
        void AddNomenclatureForSupplier(Guid soItemId);
        NomenclatureAlternativeVm GetAlternativeById(Guid id);
        void UpdateAlternative(NomenclatureAlternativeVm model);
    }

    public class NomenclatureDetailsDataItem
    {
        public Guid Id { get; set; }

        public int ClientType { get; set; }
        public string ClientName { get; set; }

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

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryFullName { get; set; }
    }

    public class NomenclatureVm
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
        public string CategoryFullName { get; set; }
    }

    public class NomenclatureAlternativeVm
    {
        public Guid Id { get; set; }

        public ClientType ClientType { get; set; }
        public string ClientName { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public Guid? BatchUomId { get; set; }
        public UomVm BatchUom { get; set; }

        public Guid? MassUomId { get; set; }
        public UomVm MassUom { get; set; }

        public decimal MassUomValue { get; set; }
       
        public Guid? ResourceUomId { get; set; }
        public UomVm ResourceUom { get; set; }

        public decimal ResourceUomValue { get; set; }

        public Guid? ResourceBatchUomId { get; set; }
        public UomVm ResourceBatchUom { get; set; }
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
        }

        public List<AutocompleteResultItem> Items { get; set; } = new List<AutocompleteResultItem>();
    }
}
