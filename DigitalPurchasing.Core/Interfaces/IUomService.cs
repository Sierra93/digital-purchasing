using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IConversionRateService
    {
        Task<UomConversionRateResponse> GetRate(Guid fromUomId, Guid nomenclatureId, Guid? customerId,
            Guid? supplierId);
    }

    public class GetRateOptions
    {
        public Guid FromUom { get; set; }
        public Guid ToUom { get; set; }

        public decimal Mass { get; set; }

        public Guid MassUom { get; set; }
    }
    
    public interface IUomService
    {
        UomIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        UomFactorData GetFactorData(Guid uomId, int page, int perPage, string sortField, bool sortAsc);
        Task<UomDto> Create(Guid ownerId, string name, decimal? quantity = null);
        Task<UomDto> Create(Guid companyId, string name, List<string> alternativeNames, decimal? quantity = null);
        UomDto CreateOrUpdate(string name);
        IEnumerable<UomDto> GetAll();
        IEnumerable<UomDto> GetByNames(params string[] uomNames);
        UomAutocompleteResponse Autocomplete(string s, Guid ownerId);
        BaseResult<UomAutocompleteResponse.AutocompleteItem> AutocompleteSingle(Guid id);
        void SaveConversionRate(
            Guid ownerId,
            Guid fromUomId,
            Guid toUomId,
            Guid? nomenclatureAlternativeId,
            decimal factorC,
            decimal factorN);
        void Delete(Guid id);
        void Delete(List<Guid> ids, Guid ownerId);
        UomDto GetById(Guid id);
        UomDto Update(Guid id, string name);
        UomDto Update(Guid id, string name, List<string> alternativeNames);
        void DeleteConversionRate(Guid id);

        Task SetPackagingUom(Guid ownerId, Guid uomId);
        Task<Guid> GetPackagingUomId(Guid ownerId);
        Task<UomDto> GetPackagingUom(Guid ownerId);

        Task SetMassUom(Guid ownerId, Guid uomId);
        Task<Guid> GetMassUomId(Guid ownerId);
        Task<UomDto> GetMassUom(Guid ownerId);

        Task SetResourceUom(Guid ownerId, Guid uomId);
        Task<Guid> GetResourceUomId(Guid ownerId);
        Task<UomDto> GetResourceUom(Guid ownerId);

        Task SetResourceBatchUom(Guid ownerId, Guid uomId);
        Task<Guid> GetResourceBatchUomId(Guid ownerId);
        Task<UomDto> GetResourceBatchUom(Guid ownerId);

        IEnumerable<string> GetAllNormalizedNames(Guid ownerId);
    }

    public class UomDto
    {
        public class JsonData
        {
            public class UomAlternativeName
            {
                public string Name { get; set; }
                public string NormalizedName { get; set; }
            }

            public List<UomAlternativeName> AlternativeNames { get; set; } = new List<UomAlternativeName>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public decimal? Quantity { get; set; }
        public JsonData Json { get; set; } = new JsonData();
    }

    public class UomAutocompleteDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public decimal? Quantity { get; set; }
        public string AlternativeName { get; set; }
    }



    public class UomFactorDataItem
    {
        public Guid Id { get; set; }
        public string Uom { get; set; }
        public decimal Factor { get; set; }
        public string Nomenclature { get; set; }
        public Guid FromId { get; set; }
        public Guid ToId { get; set; }
        public Guid? NomenclatureId { get; set; }
        public Guid? NomenclatureAlternativeId { get; set; }
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
        [DebuggerDisplay("{Id} = {Name}")]
        public class AutocompleteItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public bool IsFullMatch { get; set; }
        }

        public List<AutocompleteItem> Items = new List<AutocompleteItem>();
    }
}
