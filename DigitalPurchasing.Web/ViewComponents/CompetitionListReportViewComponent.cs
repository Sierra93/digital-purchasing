using DigitalPurchasing.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPurchasing.Web.ViewComponents
{
    public class CompetitionListReportViewComponent : ViewComponent
    {
        private readonly ISelectedSupplierService _selectedSupplierService;

        public CompetitionListReportViewComponent(
            ISelectedSupplierService selectedSupplierService)
        {
            _selectedSupplierService = selectedSupplierService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid clId)
        {
            var items = await _selectedSupplierService.GetReports(clId);
            return View(items);
        }
    }
}
