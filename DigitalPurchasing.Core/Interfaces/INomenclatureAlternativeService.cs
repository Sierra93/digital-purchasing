using System;
using System.Collections.Generic;
using System.Text;
using DigitalPurchasing.Core.Enums;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface INomenclatureAlternativeService
    {
        NomenclatureAlternativeVm GetAlternativeById(Guid id);
        void UpdateAlternative(NomenclatureAlternativeVm model);

        Guid? AddNomenclatureForCustomer(Guid prItemId);
        Guid? AddNomenclatureForSupplier(Guid soItemId);

        Guid AddOrUpdateNomenclatureAlts(Guid ownerId, Guid clientId, ClientType clientType,
            Guid nomenclatureId, string name, string code, Guid? batchUomId);

        List<Guid> AddOrUpdateNomenclatureAlts(Guid ownerId, Guid clientId, ClientType clientType,
            List<AddOrUpdateAltDto> alts);

        void Delete(Guid id);
        NomenclatureAlternativeVm GetForCustomer(Guid customerId, Guid nomenclatureId);
        NomenclatureAlternativeVm GetForSupplier(Guid supplierId, Guid nomenclatureId);
    }

    public class AddOrUpdateAltDto
    {
        public Guid NomenclatureId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public Guid? BatchUomId { get; set; }
        public Guid? MassUomId { get; set; }
        public decimal MassUomValue { get; set; }
        public Guid? ResourceUomId { get; set; }
        public decimal ResourceUomValue { get; set; }
        public Guid? ResourceBatchUomId { get; set; }
        public Guid? PackUomId { get; set; }
        public decimal? PackUomValue { get; set; }
    }

    public class NomenclatureAlternativeVm
    {
        public Guid Id { get; set; }

        public ClientType ClientType { get; set; }
        public string ClientName { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public Guid? BatchUomId { get; set; }
        public UomDto BatchUom { get; set; }

        public Guid? MassUomId { get; set; }
        public UomDto MassUom { get; set; }

        public decimal MassUomValue { get; set; }

        public Guid? ResourceUomId { get; set; }
        public UomDto ResourceUom { get; set; }

        public decimal ResourceUomValue { get; set; }

        public Guid? ResourceBatchUomId { get; set; }
        public UomDto ResourceBatchUom { get; set; }

        public Guid? PackUomId { get; set; }
        public decimal? PackUomValue { get; set; }

        public Guid NomenclatureId { get; set; }
    }
}
