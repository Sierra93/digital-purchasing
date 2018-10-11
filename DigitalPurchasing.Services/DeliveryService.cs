using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;

namespace DigitalPurchasing.Services
{
    public class DeliveryService: IDeliveryService
    {
        private readonly ApplicationDbContext _db;

        public DeliveryService(ApplicationDbContext db) => _db = db;

        public DeliveryVm GetByPrId(Guid prId)
        {
            var result = DefaultResult();
            var pr = _db.PurchaseRequests.Find(prId);
            if (pr?.DeliveryId == null) return result;
            _db.Entry(pr).Reference(q => q.Delivery).Load();
            return pr.Delivery.Adapt(result);
        }

        public DeliveryVm GetByQrId(Guid prId)
        {
            var result = DefaultResult();
            var pr = _db.QuotationRequests.Find(prId);
            if (pr?.DeliveryId == null) return result;
            _db.Entry(pr).Reference(q => q.Delivery).Load();
            return pr.Delivery.Adapt(result);
        }

        private DeliveryVm DefaultResult() => new DeliveryVm {
            DeliveryTermsOptions = GetDeliveryTermsOptions(),
            PaymentTermsOptions = GetPaymentTermsOptions(),
            DeliverAt = DateTime.UtcNow
        };

        private IEnumerable<DeliveryVm.Option> GetDeliveryTermsOptions()
            => Enum.GetValues(typeof(DeliveryTerms)).OfType<DeliveryTerms>().Select(value => new DeliveryVm.Option { Text = value.GetDescription(), Value = (int)value });

        private IEnumerable<DeliveryVm.Option> GetPaymentTermsOptions()
            => Enum.GetValues(typeof(PaymentTerms)).OfType<PaymentTerms>().Select(value => new DeliveryVm.Option { Text = value.GetDescription(), Value = (int)value });

        public void CreateOrUpdate(DeliveryVm req, Guid? prId, Guid? qrId)
        {
            var delivery = prId.HasValue ? GetByPrId(prId.Value) : qrId.HasValue ? GetByQrId(qrId.Value) : null;
            if (delivery == null)
            {
                throw new ApplicationException("no delivery");
            }
            if (delivery.Id == Guid.Empty)
            {
                var newEntity = req.Adapt<Delivery>();
                var entry = _db.Deliveries.Add(newEntity);
                _db.SaveChanges();
                if (prId.HasValue)
                {
                    _db.PurchaseRequests.Find(prId.Value).DeliveryId = entry.Entity.Id;
                }
                if (qrId.HasValue)
                {
                    _db.QuotationRequests.Find(qrId.Value).DeliveryId = entry.Entity.Id;
                }
            }
            else
            {
                var entity = _db.Deliveries.Find(delivery.Id);
                entity = req.Adapt(entity);
            }
            _db.SaveChanges();
        }
    }

    public class DeliveryMappings : IRegister
    {
        public void Register(TypeAdapterConfig config) =>
            config.NewConfig<DeliveryVm, Delivery>()
                .Ignore(q => q.Id)
                .Ignore(q => q.OwnerId)
                .Ignore(q => q.CreatedOn);
    }
}
