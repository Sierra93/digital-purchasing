using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;

namespace DigitalPurchasing.Services
{
    public class CustomerService : ClientService, ICustomerService
    {
        public CustomerService(ApplicationDbContext db) : base(db) {}

        public ClientAutocompleteVm Autocomplete(AutocompleteBaseOptions options) =>
            Autocomplete(options, ClientType.Customer);
    }
}