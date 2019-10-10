using System;
using DigitalPurchasing.Core.Extensions;
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
                var delivery = _deliveryService.GetByPrId(prId.Value);
                delivery.DeliverAt = User.ToLocalTime(delivery.DeliverAt);
                return Ok(delivery);
            }
            if (qrId.HasValue)
            {
                var delivery = _deliveryService.GetByQrId(qrId.Value);
                delivery.DeliverAt = User.ToLocalTime(delivery.DeliverAt);
                return Ok(delivery);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Save([FromBody]DeliveryVm req, [FromQuery]Guid? prId, [FromQuery]Guid? qrId)
        {
            req.DeliverAt = User.ToUtcTime(req.DeliverAt);
            _deliveryService.CreateOrUpdate(req, prId, qrId);
            return Ok();
        }
    }
}
