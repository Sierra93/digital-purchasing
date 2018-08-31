using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class ColumnController : BaseController
    {
        private readonly IColumnNameService _columnNameService;

        public ColumnController(IColumnNameService columnNameService) => _columnNameService = columnNameService;

        public IActionResult Index() => View();

        public IActionResult Data()
        {
            var names = _columnNameService.GetAllNames();
            return Json(names);
        }
    }
}
