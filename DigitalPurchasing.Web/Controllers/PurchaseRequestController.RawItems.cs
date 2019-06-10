using System;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.ViewModels.PurchasingRequest;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public partial class PurchaseRequestController
    {
        [HttpGet]
        public IActionResult RawItemsData(Guid id)
        {
            var response = _purchasingRequestService.GetRawItems(id);
            return Json(response);
        }

        [HttpPost]
        public IActionResult SaveRawItemsData([FromBody] SaveRawItemsDataVm model)
        {
            var id = model.PurchasingRequestId;
            var userId = Guid.Parse(_userManager.GetUserId(User));
            _purchasingRequestService.SaveCompanyName(id, _companyService.GetByUser(userId).Name);
            _purchasingRequestService.SaveRawItems(id, model.Items);
            _purchasingRequestService.UpdateStatus(id, PurchaseRequestStatus.MatchItems);
            return Ok();
        }

        [HttpPost]
        public IActionResult SaveCustomerName([FromBody] SaveCustomerNameVm model)
        {
            _purchasingRequestService.SaveCustomerName(model.Id, model.Name, model.CustomerId);
            return Ok();
        }
    }
}
