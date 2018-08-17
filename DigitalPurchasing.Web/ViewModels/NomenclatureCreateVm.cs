using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalPurchasing.Web.ViewModels
{
    public class NomenclatureCreateVm
    {
        public string Code { get; set; }

        public string Name { get; set; }
        public string NameEng { get; set; }

        public Guid BatchUomId { get; set; }
        public List<SelectListItem> BatchUoms { get; set; }

        public Guid MassUomId { get; set; }
        public List<SelectListItem> MassUoms { get; set; }
        public decimal MassUomValue { get; set; }
       
        public Guid ResourceUomId { get; set; }
        public List<SelectListItem> ResourceUoms { get; set; }
        public decimal ResourceUomValue { get; set; }

        public Guid ResourceBatchUomId { get; set; }
        public List<SelectListItem> ResourceBatchUoms { get; set; }

        public Guid CategoryId { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
