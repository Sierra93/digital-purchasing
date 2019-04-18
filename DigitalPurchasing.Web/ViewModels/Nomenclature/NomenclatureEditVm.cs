using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DigitalPurchasing.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace DigitalPurchasing.Web.ViewModels
{
    public class NomenclatureEditVm
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Код")]
        public string Code { get; set; }

        [Required, DisplayName("Наименование")]
        public string Name { get; set; }

        [DisplayName("Наименование (eng)")]
        public string NameEng { get; set; }

        [DisplayName("ЕИ")]
        public Guid BatchUomId { get; set; }
        public List<SelectListItem> BatchUoms { get; set; }

        [DisplayName("ЕИ массы")]
        public Guid MassUomId { get; set; }
        public List<SelectListItem> MassUoms { get; set; }

        [DisplayName("Масса 1 ЕИ")]
        public decimal MassUomValue { get; set; }

        [DisplayName("Название ресурса")]
        public Guid ResourceUomId { get; set; }
        public List<SelectListItem> ResourceUoms { get; set; }
        [DisplayName("Ресурс, 1 ЕИ ресурса")]
        public decimal ResourceUomValue { get; set; }

        [DisplayName("ЕИ ресурса")]
        public Guid ResourceBatchUomId { get; set; }
        public List<SelectListItem> ResourceBatchUoms { get; set; }

        [DisplayName("ЕИ товара в упаковке")]
        public Guid? PackUomId { get; set; }
        public List<SelectListItem> PackUoms { get; set; }
        [DisplayName("Количество товара в упаковке")]
        public decimal PackUomValue { get; set; }

        [DisplayName("Категория")]
        public Guid CategoryId { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
