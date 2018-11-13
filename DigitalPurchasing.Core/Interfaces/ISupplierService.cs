using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ISupplierService
    {
        ClientAutocompleteVm Autocomplete(AutocompleteBaseOptions options);
    }

    public class ClientAutocompleteVm
    {
        public class ClientVm
        {
            public string Name { get; set; }
        }

        public List<ClientVm> Items { get; set; } = new List<ClientVm>();
    }
}
