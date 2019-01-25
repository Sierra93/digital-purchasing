using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICustomerService
    {
        CustomerAutocomplete Autocomplete(AutocompleteBaseOptions options);
        Guid CreateCustomer(string name);
        string GetNameById(Guid id);
        CustomerVm GetById(Guid id);
    }

    public class CustomerAutocomplete
    {
        public class Customer
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
        }

        public List<Customer> Items { get; set; } = new List<Customer>();
    }

    public class CustomerVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
