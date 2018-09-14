using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models;
using Mapster;

namespace DigitalPurchasing.Mappings
{
    public class PurchasingRequestItemMappings : IRegister
    {
        public void Register(TypeAdapterConfig config) =>
            config.NewConfig<PurchasingRequestItem, MatchItemsResponse.Item>()
                .Map(d => d.NomenclatureUom, s => s.NomenclatureId.HasValue ? s.Nomenclature.BatchUom.Name : null)
                .Map(q => q.RawUom, q => q.RawUomMatchId.HasValue ? q.RawUomMatch.Name : q.RawUom);
    }
}
