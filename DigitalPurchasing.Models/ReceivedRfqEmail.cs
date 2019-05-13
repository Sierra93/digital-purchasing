using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public sealed class ReceivedRfqEmail : ReceivedEmail
    {
        public Guid QuotationRequestId { get; set; }
        public QuotationRequest QuotationRequest { get; set; }
    }
}
