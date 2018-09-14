using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models;
using Mapster;

namespace DigitalPurchasing.Mappings
{
    public class NomenclatureCategoryMappings : IRegister
    {
        public void Register(TypeAdapterConfig config) => config.NewConfig<NomenclatureCategory, NomenclatureCategoryResult>().Map(d => d.ParentName, s => s.Parent != null ? s.Parent.Name : null);
    }
}
