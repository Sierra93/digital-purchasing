namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICustomerService
    {
        ClientAutocompleteVm Autocomplete(AutocompleteBaseOptions options);
    }
}
