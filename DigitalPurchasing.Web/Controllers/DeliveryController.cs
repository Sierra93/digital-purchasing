using System;
using DigitalPurchasing.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveryController(IDeliveryService deliveryService) => _deliveryService = deliveryService;

        [HttpGet]
        public IActionResult Load([FromQuery]Guid? prId, [FromQuery]Guid? qrId)
        {
            if (prId.HasValue)
            {
                return Ok(_deliveryService.GetByPrId(prId.Value));
            }
            if (qrId.HasValue)
            {
                return Ok(_deliveryService.GetByQrId(qrId.Value));
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Save([FromBody]DeliveryVm req, [FromQuery]Guid? prId, [FromQuery]Guid? qrId)
        {
            _deliveryService.CreateOrUpdate(req, prId, qrId);
            return Ok();
        }
    }
}
