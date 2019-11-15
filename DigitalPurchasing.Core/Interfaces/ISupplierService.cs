using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ISupplierService
    {
        SupplierAutocomplete Autocomplete(AutocompleteBaseOptions options, bool includeCategories);
        Guid CreateSupplier(string name, Guid ownerId);
        Guid CreateSupplier(SupplierVm model, Guid ownerId);
        string GetNameById(Guid id);
        SupplierIndexData GetData(int page, int perPage, string sortField, bool sortAsc, string search);
        SupplierVm GetById(Guid id);
        SupplierVm GetById(Guid id, bool globalSearch);
        IEnumerable<SupplierVm> GetByPublicIds(params int[] publicIds);
        void Update(SupplierVm model);

        List<SupplierContactPersonVm> GetContactPersonsBySupplier(Guid supplierId, bool whichCouldBeUsedForRequestsOnly = false);
        Guid AddContactPerson(SupplierContactPersonVm vm);
        Guid EditContactPerson(SupplierContactPersonVm vm);
        SupplierContactPersonVm GetContactPersonsById(Guid personId);
        void DeleteContactPerson(Guid personId);
        (Guid SupplierId, Guid ContactPersonId) GetSupplierIdByEmail(Guid ownerId, string email);
        string GetSupplierNameByEmail(Guid ownerId, string email);
        List<SupplierNomenclatureCategory> GetSupplierNomenclatureCategories(Guid supplierId);
        Dictionary<SupplierVm, List<Guid>> GetByCategoryIds(IList<Guid> nomenclatureCategoryIds, IList<Guid> ignoreSupplierId);
        void SaveSupplierNomenclatureCategoryContacts(Guid supplierId,
            IEnumerable<(Guid NomenclatureCategoryId, Guid? PrimarySupplierContactId, Guid? SecondarySupplierContactId, bool IsDefaultSupplierCategory)> valueTuples);
        void RemoveSupplierNomenclatureCategoryContacts(Guid supplierId, Guid nomenclatureCategoryId);
        void RemoveSupplierNomenclatureCategories(Guid supplierId);
    }

    public class SupplierAutocomplete
    {
        public class Supplier
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
            public List<SupplierNomenclatureCategory> Categories { get; set; }
        }

        public List<Supplier> Items { get; set; } = new List<Supplier>();
    }

    public class SupplierIndexDataItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MainCategoriesCsv { get; set; }
    }

    public class SupplierIndexData : BaseDataResponse<SupplierIndexDataItem>
    {
    }

    public class SupplierVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OwnershipType { get; set; }
        public long? Inn { get; set; }
        public string ErpCode { get; set; }
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
        public bool PriceWithVat { get; set; }
        public bool SumWithVat { get; set; }
        public string SupplierType { get; set; }
        public int? PaymentDeferredDays { get; set; }
        public string DeliveryTerms { get; set; }
        public string OfferCurrency { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public Guid? CategoryId { get; set; }
        public int PublicId { get; set; }

        public Guid OwnerId { get; set; }
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
        public string MobilePhoneNumber { get; set; }

        public Guid SupplierId { get; set; }

        public bool UseForRequests { get; set; }

        public string FullName => $"{LastName??""} {FirstName??""} {Patronymic??""}".Trim();

        public string ToName()
        {
            var toName = FirstName;

            if (!string.IsNullOrWhiteSpace(Patronymic))
            {
                toName += $" {Patronymic}";
            }

            return toName;
        }
    }

    public class SupplierNomenclatureCategory
    {
        public Guid? NomenclatureCategoryId { get; set; }
        public string NomenclatureCategoryFullName { get; set; }
        public Guid? NomenclatureCategoryPrimaryContactId { get; set; }
        public Guid? NomenclatureCategorySecondaryContactId { get; set; }
        public bool IsDefaultSupplierCategory { get; set; }
    }
}
