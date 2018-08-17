using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalPurchasing.Web.Core
{
    public interface IDictionaryService
    {
        List<SelectListItem> GetCategories(Guid? exceptId = null);
        List<SelectListItem> GetUoms(Guid? exceptId = null);
    }

    public class DictionaryService : IDictionaryService
    {
        private readonly INomenclatureCategoryService _categoryService;
        private readonly IUomService _uomService;

        public DictionaryService(INomenclatureCategoryService categoryService, IUomService uomService)
        {
            _categoryService = categoryService;
            _uomService = uomService;
        }

        public List<SelectListItem> GetCategories(Guid? exceptId = null)
        {
            var allCategories = _categoryService.GetAll();
            if (exceptId.HasValue)
            {
                allCategories = allCategories.Where(q => q.Id != exceptId.Value);
            }

            return allCategories.Select(q => new SelectListItem(q.Name, q.Id.ToString("N"))).ToList();
        }

        public List<SelectListItem> GetUoms(Guid? exceptId = null)
        {
            var allUoms = _uomService.GetAll();
            if (exceptId.HasValue)
            {
                allUoms = allUoms.Where(q => q.Id != exceptId.Value);
            }

            return allUoms.Select(q => new SelectListItem(q.Name, q.Id.ToString("N"))).ToList();
        }
    }
}
