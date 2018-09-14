using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models;
using Mapster;

namespace DigitalPurchasing.Services
{
    public class UomResultMapsterRegister : IRegister
    {
        public void Register(TypeAdapterConfig config) => config.NewConfig<UnitsOfMeasurement, UomResult>().Map(d => d.IsSystem, s => !s.OwnerId.HasValue);
    }
}