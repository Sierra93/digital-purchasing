using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models;
using Mapster;

namespace DigitalPurchasing.Mappings
{
    public class UnitsOfMeasurementMappings : IRegister
    {
        public void Register(TypeAdapterConfig config) => config.NewConfig<UnitsOfMeasurement, UomResult>().Map(d => d.IsSystem, s => !s.OwnerId.HasValue);
    }
}
