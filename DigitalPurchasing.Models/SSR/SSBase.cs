using System;

namespace DigitalPurchasing.Models.SSR
{
    public abstract class SSBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
