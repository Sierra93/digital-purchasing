using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface INomenclatureAlternativeService
    {
        NomenclatureAlternativeVm GetAlternativeById(Guid id);
        void UpdateAlternative(NomenclatureAlternativeVm model);

        void AddNomenclatureForCustomer(Guid prItemId);
        void AddNomenclatureForSupplier(Guid soItemId);

        void AddOrUpdateNomenclatureAlts(Guid ownerId, Guid clientId, ClientType clientType,
            Guid nomenclatureId, string name, string code, Guid? batchUomId);

        void AddOrUpdateNomenclatureAlts(Guid ownerId, Guid clientId, ClientType clientType,
            List<(Guid NomenclatureId, string Name, string Code, Guid? BatchUomId, Guid? MassUomId, Guid? ResourceBatchUomId, Guid? ResourceUomId)> alts);

        void AddOrUpdateNomenclatureAlts(Guid ownerId, Guid clientId, ClientType clientType,
            List<(Guid NomenclatureId, string Name, string Code, Guid? BatchUomId)> alts);
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

        public Guid NomenclatureId { get; set; }
    }
}
