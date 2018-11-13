using System;
using System.Linq;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;

namespace DigitalPurchasing.Services
{
    public abstract class ClientService
    {
        private readonly ApplicationDbContext _db;
        private const StringComparison StrComparison = StringComparison.InvariantCultureIgnoreCase;

        protected ClientService(ApplicationDbContext db) => _db = db;

        protected ClientAutocompleteVm Autocomplete(AutocompleteBaseOptions options, ClientType clientType)
        {
            var data = _db.NomenclatureAlternatives
                .Where(q => q.ClientName.Contains(options.Query, StrComparison) && q.ClientType == clientType)
                .OrderBy(q => q.ClientName)
                .Select(q => q.ClientName)
                .Distinct()
                .ToList();

            var result = new ClientAutocompleteVm();

            if (data.Any())
            {
                result.Items = data
                    .Select(q => new ClientAutocompleteVm.ClientVm { Name = q })
                    .ToList();
            }

            return result;
        }

    }
}
