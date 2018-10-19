using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalPurchasing.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalPurchasing.Web.ViewModels.Nomenclature
{
    public class NomenclatureAlternativeEditVm
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [Required, DisplayName("Тип организации")]
        public ClientType ClientType { get; set; }

        [Required, DisplayName("Название организации")]
        public string ClientName { get; set; }

        [DisplayName("Код")]
        public string Code { get; set; }

        [Required, DisplayName("Наименование")]
        public string Name { get; set; }

        [DisplayName("ЕИ")]
        public Guid? BatchUomId { get; set; }
        public List<SelectListItem> BatchUoms { get; set; }

        [DisplayName("ЕИ массы")]
        public Guid? MassUomId { get; set; }
        public List<SelectListItem> MassUoms { get; set; }

        [DisplayName("Масса 1 ЕИ")]
        public decimal MassUomValue { get; set; }

        [DisplayName("Название ресурса")]
        public Guid? ResourceUomId { get; set; }
        public List<SelectListItem> ResourceUoms { get; set; }
        [DisplayName("Ресурс, 1 ЕИ ресурса")]
        public decimal ResourceUomValue { get; set; }

        [DisplayName("ЕИ ресурса")]
        public Guid? ResourceBatchUomId { get; set; }
        public List<SelectListItem> ResourceBatchUoms { get; set; }
    }
}
