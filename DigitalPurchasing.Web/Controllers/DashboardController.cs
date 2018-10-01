using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IPurchaseRequestService _purchasingRequestService;

        public DashboardController(IPurchaseRequestService purchasingRequestService)
        {
            _purchasingRequestService = purchasingRequestService;
        }

        public IActionResult Index() => View();
    }
}
