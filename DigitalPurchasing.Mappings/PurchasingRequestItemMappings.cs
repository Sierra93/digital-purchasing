using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models;
using Mapster;

namespace DigitalPurchasing.Mappings
{
    public class PurchaseRequestItemMappings : IRegister
    {
        public void Register(TypeAdapterConfig config) =>
            config.NewConfig<PurchaseRequestItem, MatchItemsResponse.Item>()
                .Map(d => d.NomenclatureUom, s => s.NomenclatureId.HasValue ? s.Nomenclature.BatchUom.Name : null)
                .Map(q => q.RawUom, q => q.RawUomMatchId.HasValue ? q.RawUomMatch.Name : q.RawUom);
    }
}
