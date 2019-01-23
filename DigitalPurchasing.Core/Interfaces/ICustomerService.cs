using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICustomerService
    {
        ClientAutocompleteVm Autocomplete(AutocompleteBaseOptions options);
    }

    public class ClientAutocompleteVm
    {
        public class ClientVm
        {
            public string Name { get; set; }
        }

        public List<ClientVm> Items { get; set; }
    }
}
