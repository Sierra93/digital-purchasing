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
        public string PhoneNumber { get; set; }

        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}
