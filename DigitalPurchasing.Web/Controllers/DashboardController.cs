using System;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
            => _dashboardService = dashboardService;

        public async Task<IActionResult> Index()
        {
            var companyId = User.CompanyId();
            var nowDate = DateTime.UtcNow.Date;
            var daysInMonth = DateTime.DaysInMonth(nowDate.Year, nowDate.Month);
            var fromDate = new DateTime(nowDate.Year, nowDate.Month, 1, 0, 0, 0);
            var toDate = new DateTime(nowDate.Year, nowDate.Month, daysInMonth, 23, 59, 59);
            var requestStatuses = await _dashboardService.GetRequestStatuses(companyId, fromDate, toDate);
            var suppliers = await _dashboardService.GetTopSuppliers(companyId, fromDate, toDate);

            var vm = new DashboardIndexVm
            {
                FromDate = fromDate,
                ToDate = toDate,
                RequestStatuses = requestStatuses,
                Suppliers = suppliers
            };

            return View(vm);
        }
    }
}
