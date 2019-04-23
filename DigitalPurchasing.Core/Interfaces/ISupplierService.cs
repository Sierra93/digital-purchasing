using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ISupplierService
    {
        SupplierAutocomplete Autocomplete(AutocompleteBaseOptions options);
        Guid CreateSupplier(string name);
        string GetNameById(Guid id);
        SupplierIndexData GetData(int page, int perPage, string sortField, bool sortAsc, string search);
        SupplierVm GetById(Guid id);
        SupplierVm GetById(Guid id, bool globalSearch);
        void Update(SupplierVm model);

        List<SupplierContactPersonVm> GetContactPersonsBySupplier(Guid supplierId);
        Guid AddContactPerson(SupplierContactPersonVm vm);
        Guid EditContactPerson(SupplierContactPersonVm vm);
        SupplierContactPersonVm GetContactPersonsById(Guid personId);
        void DeleteContactPerson(Guid personId);
        SupplierContactPersonVm GetContactPersonBySupplier(Guid supplierId);
        Guid GetSupplierByEmail(string email);
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
        public string OwnershipType { get; set; }
        public string Inn { get; set; }
        public string ErpCode { get; set; }
        public string Code { get; set; }
        public string Website { get; set; }
        public string LegalAddressStreet { get; set; }
        public string LegalAddressCity { get; set; }
        public string LegalAddressCountry { get; set; }
        public string ActualAddressStreet { get; set; }
        public string ActualAddressCity { get; set; }
        public string ActualAddressCountry { get; set; }
        public string WarehouseAddressStreet { get; set; }
        public string WarehouseAddressCity { get; set; }
        public string WarehouseAddressCountry { get; set; }

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
