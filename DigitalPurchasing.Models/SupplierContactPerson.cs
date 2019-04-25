using System;

namespace DigitalPurchasing.Models
{
    public class SupplierContactPerson : BaseModelWithOwner
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string JobTitle { get; set; }
        public string Patronymic { get; set; }
        public string MobilePhoneNumber { get; set; }

        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public bool UseForRequests { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

    //public class SupplierContactPersonCategory : BaseModel
    //{
    //    public SupplierContactPerson ContactPerson { get; set; }
    //    public Guid ContactPersonId { get; set; }

    //    public SupplierContactPerson ContactPerson { get; set; }
    //    public Guid ContactPersonId { get; set; }

    //    public NomenclatureCategory NomenclatureCategory { get; set; }
    //    public Guid NomenclatureCategoryId { get; set; }
    //}
}
