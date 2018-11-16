using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IDeliveryService
    {
        DeliveryVm GetByPrId(Guid prId);
        DeliveryVm GetByQrId(Guid prId);
        void CreateOrUpdate(DeliveryVm req, Guid? prId, Guid? qrId);
        DeleteResultVm Delete(Guid id);
    }

    public class DeliveryVm
    {
        public class Option
        {
            public string Text { get; set; }
            public int Value { get; set; }
        }

        public Guid Id { get; set; }

        public DateTime DeliverAt { get; set; }
        public DeliveryTerms DeliveryTerms { get; set; }
        public PaymentTerms PaymentTerms { get; set; }

        public int PayWithinDays { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        public string House { get; set; }
        public string Building { get; set; }
        public string Structure { get; set; }
        public string OfficeOrApartment { get; set; }

        public IEnumerable<Option> DeliveryTermsOptions { get; set; }
        public IEnumerable<Option> PaymentTermsOptions { get; set; }
    }

    public class DeliveryCreateUpdateRequest: DeliveryVm
    { 
        public Guid? PrId { get; set; }
        public Guid? QrId { get; set; }
    }
}
