using System;
using System.Collections.Generic;
using System.Text;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models;
using Mapster;
using Newtonsoft.Json;

namespace DigitalPurchasing.Services.Mappings
{
    public class PriceReductionEmailMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
            => config.NewConfig<PriceReductionEmail, PriceReductionEmailDto>()
                .Map(q => q.Data, q => JsonConvert.DeserializeObject<List<Guid>>(q.Data));
    }
}
