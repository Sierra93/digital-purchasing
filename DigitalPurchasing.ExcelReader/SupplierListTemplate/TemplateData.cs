using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.ExcelReader.SupplierListTemplate
{
    public class TemplateData
    {
        public string SupplierName { get; set; }
        public string MainCategory { get; set; }
        public string SubCategory1 { get; set; }
        public string SubCategory2 { get; set; }
        public string OwnershipType { get; set; }
        public long? Inn { get; set; }
        public string ErpCode { get; set; }
        public string SupplierType { get; set; }
        public string PaymentDeferredDays { get; set; }
        public string DeliveryTerms { get; set; }
        public string OfferCurrency { get; set; }
        public bool PriceWithVat { get; set; }
        public string Website { get; set; }
        public string SupplierPhone { get; set; }

        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactJobTitle { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactMobilePhone { get; set; }
        public bool ContactSpecified => !string.IsNullOrWhiteSpace(ContactFirstName) || !string.IsNullOrWhiteSpace(ContactLastName) ||
            !string.IsNullOrWhiteSpace(ContactJobTitle) || !string.IsNullOrWhiteSpace(ContactEmail) ||
            !string.IsNullOrWhiteSpace(ContactMobilePhone);

        public string Note { get; set; }
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
}
