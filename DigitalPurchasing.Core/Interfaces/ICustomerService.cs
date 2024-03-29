using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICustomerService
    {
        CustomerAutocomplete Autocomplete(AutocompleteBaseOptions options);
        Guid CreateCustomer(string name, Guid ownerId);
        string GetNameById(Guid id);
        CustomerVm GetById(Guid id);
        CustomerIndexData GetData(int page, int perPage, string sortField, bool sortAsc, string search);
        void Update(CustomerVm model);
        void Delete(Guid id);
        IEnumerable<CustomerVm> GetByPublicIds(params int[] publicIds);
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
        public int PublicId { get; set; }
    }

    public class CustomerIndexData : BaseDataResponse<CustomerIndexDataItem>
    {
    }

    public class CustomerIndexDataItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
