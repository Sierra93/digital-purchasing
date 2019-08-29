using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalPurchasing.Web.ViewModels.Nomenclature
{
    public class NomenclatureAlternativeEditVm
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Тип организации")]
        public ClientType ClientType { get; set; }

        public string ClientTypeStr => ClientType.GetDescription();

        [DisplayName("Название организации")]
        public string ClientName { get; set; }

        [DisplayName("Код")]
        public string Code { get; set; }

        [Required, DisplayName("Наименование")]
        public string Name { get; set; }

        [DisplayName("ЕИ")]
        public Guid? BatchUomId { get; set; }
        public string BatchUomName { get; set; }

        [DisplayName("ЕИ массы")]
        public Guid? MassUomId { get; set; }
        public string MassUomName { get; set; }

        [DisplayName("Масса 1 ЕИ")]
        public decimal MassUomValue { get; set; }

        [DisplayName("Название ресурса")]
        public Guid? ResourceUomId { get; set; }
        public string ResourceUomName { get; set; }
        [DisplayName("Ресурс, 1 ЕИ ресурса")]
        public decimal ResourceUomValue { get; set; }

        [DisplayName("ЕИ ресурса")]
        public Guid? ResourceBatchUomId { get; set; }
        public string ResourceBatchUomName { get; set; }

        [DisplayName("ЕИ товара в упаковке")]
        public Guid? PackUomId { get; set; }
        [DisplayName("Количество товара в упаковке")]
        public decimal? PackUomValue { get; set; }
        public string PackUomName { get; set; }

        public List<SelectListItem> EmptyList { get; set; } = new List<SelectListItem>();
    }
}
