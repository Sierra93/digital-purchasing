using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public partial class PurchasingRequestController
    {
        public IActionResult ColumnsData(Guid id)
        {
            var response = _purchasingRequestService.GetColumnsById(id);
            return Json(response);
        }

        [HttpPost]
        public IActionResult SaveColumnsData([FromBody]SavePurchasingRequestColumnsVm model)
        {
            var id = model.PurchasingRequestId;
            _purchasingRequestService.SaveColumns(id, model);
            _purchasingRequestService.GenerateRawItems(id);
            _purchasingRequestService.UpdateStatus(id, PurchasingRequestStatus.ManualInput);
            return Ok();
        }
    }
}
