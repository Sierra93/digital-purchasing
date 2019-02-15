using System;

namespace DigitalPurchasing.Emails.EmailTemplates
{
    public class RFQEmail
    {
        public class FromData
        {
            public string Name { get; set; }
            public string JobTitle { get; set; }
            public string Company { get; set; }
        }

        public string ToName { get; set; }
        public DateTime Until { get; set; }
        public FromData From { get; set; }

        public RFQEmail() => From = new FromData();
    }
}
