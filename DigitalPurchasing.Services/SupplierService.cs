using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;

namespace DigitalPurchasing.Services
{
    public class SupplierService : ClientService, ISupplierService
    {
        public SupplierService(ApplicationDbContext db) : base(db) {}

        public ClientAutocompleteVm Autocomplete(AutocompleteBaseOptions options) =>
            Autocomplete(options, ClientType.Supplier);
    }
}