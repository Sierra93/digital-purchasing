using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICurrencyService
    {
        CurrencyVm GetDefaultCurrency(Guid companyId);
    }

    public class CurrencyVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
