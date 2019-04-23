using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Web.ViewModels.Supplier
{
    public class SupplierEditVm
    {
        public class SupplierVm
        {
            public Guid Id { get; set; }

            [Required]
            [Display(Name = "Наименование")]
            public string Name { get; set; }

            [Display(Name = "Фома собственности")]
            public string OwnershipType { get; set; }

            [Display(Name = "ИНН")]
            public string Inn { get; set; }

            [Display(Name = "Код во внутренней ERP")]
            public string ErpCode { get; set; }

            [Display(Name = "Код в системе")]
            public string Code { get; set; }

            [Display(Name = "Веб-сайт")]
            public string Website { get; set; }

            [Display(Name = "Улица")]
            public string LegalAddressStreet { get; set; }

            [Display(Name = "Город")]
            public string LegalAddressCity { get; set; }

            [Display(Name = "Страна")]
            public string LegalAddressCountry { get; set; }

            [Display(Name = "Улица")]
            public string ActualAddressStreet { get; set; }

            [Display(Name = "Город")]
            public string ActualAddressCity { get; set; }

            [Display(Name = "Страна")]
            public string ActualAddressCountry { get; set; }

            [Display(Name = "Улица")]
            public string WarehouseAddressStreet { get; set; }

            [Display(Name = "Город")]
            public string WarehouseAddressCity { get; set; }

            [Display(Name = "Страна")]
            public string WarehouseAddressCountry { get; set; }
        }

        public SupplierEditVm.SupplierVm Supplier { get; set; }
        public List<SupplierContactPersonVm> ContactPersons { get; set; }
    }
}
