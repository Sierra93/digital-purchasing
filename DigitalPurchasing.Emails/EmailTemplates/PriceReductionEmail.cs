using System;

namespace DigitalPurchasing.Emails.EmailTemplates
{
    public class PriceReductionEmail
    {
        public class FromData
        {
            public string Name { get; set; }
            public string Company { get; set; }
            public string Phone { get; set; }
        }

        public DateTime Until { get; set; }
        public string InvoiceData { get; set; }

        public PriceReductionEmail() => From = new FromData();

        public FromData From { get; set; }
        public string ToName { get; set; }
    }
}
