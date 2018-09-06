using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IUomService
    {
        UomDataResult GetData(int page, int perPage, string sortField, bool sortAsc);
        UomResult CreateUom(string name);
        IEnumerable<UomResult> GetAll();
        UomConversionRateResult GetConversionRate(Guid fromUomId, Guid toUomId, Guid nomenclatureId);
    }

    public class UomResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSystem { get; set; }
    }

    public class UomDataResult : BaseDataResponse<UomResult>
    {
    }

    public class UomConversionRateResult
    {
        public decimal NomenclatureFactor { get; set; }
        public decimal CommonFactor { get; set; }
    }
}
