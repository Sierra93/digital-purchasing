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
            public long? Inn { get; set; }

            [Display(Name = "Цены с НДС")]
            public bool PriceWithVat { get; set; }

            [Display(Name = "Сумма с НДС")]
            public bool SumWithVat { get; set; }

            [Display(Name = "Код поставщика в Системе")]
            public int PublicId { get; set; }

            [Display(Name = "Код во внутренней ERP")]
            public string ErpCode { get; set; }

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

            [Display(Name = "Тип поставщика")]
            public string SupplierType { get; set; }

            [Display(Name = "Отсрочка платежа, дней")]
            public int? PaymentDeferredDays { get; set; }

            [Display(Name = "Условия поставки (incoterms)")]
            public string DeliveryTerms { get; set; }

            [Display(Name = "Валюта выставления предложений")]
            public string OfferCurrency { get; set; }

            [Display(Name = "Телефон общий")]
            public string Phone { get; set; }

            [Display(Name = "Комментарии")]
            public string Note { get; set; }

            public Guid? CategoryId { get; set; }
        }

        public SupplierEditVm.SupplierVm Supplier { get; set; }
        public List<SupplierContactPersonVm> ContactPersons { get; set; } = new List<SupplierContactPersonVm>();
        public List<SupplierNomenclatureCategory> NomenclatureCategoies { get; set; } = new List<SupplierNomenclatureCategory>();
        public List<NomenclatureCategoryVm> AvailableCategories { get; set; } = new List<NomenclatureCategoryVm>();
    }
}
