using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ISupplierService
    {
        SupplierAutocomplete Autocomplete(AutocompleteBaseOptions options);
        Guid CreateSupplier(string name);
        string GetNameById(Guid id);
        SupplierIndexData GetData(int page, int perPage, string sortField, bool sortAsc);
        SupplierVm GetById(Guid id);
    }

    public class SupplierAutocomplete
    {
        public class Supplier
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
        }

        public List<Supplier> Items { get; set; } = new List<Supplier>();
    }

    public class SupplierIndexDataItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class SupplierIndexData : BaseDataResponse<SupplierIndexDataItem>
    {
    }

    public class SupplierVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
