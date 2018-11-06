using System;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using Mapster;

namespace DigitalPurchasing.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ApplicationDbContext _db;

        public CurrencyService(ApplicationDbContext db) => _db = db;

        public CurrencyVm GetDefaultCurrency(Guid companyId) => _db.Currencies.First(q => q.Name == "RUB").Adapt<CurrencyVm>();
    }
}
