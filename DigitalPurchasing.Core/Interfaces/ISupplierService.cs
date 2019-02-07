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

        List<SupplierContactPersonVm> GetContactPersonsBySupplier(Guid supplierId);
        Guid AddContactPerson(SupplierContactPersonVm vm);
        Guid EditContactPerson(SupplierContactPersonVm vm);
        SupplierContactPersonVm GetContactPersonsById(Guid personId);
        void DeleteContactPerson(Guid personId);
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

    public class SupplierContactPersonVm
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string JobTitle { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNumber { get; set; }

        public Guid SupplierId { get; set; }

        public bool UseForRequests { get; set; }

        public string FullName => $"{LastName??""} {FirstName??""} {Patronymic??""}".Trim();
    }
}
