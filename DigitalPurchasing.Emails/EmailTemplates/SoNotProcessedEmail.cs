using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Emails.EmailTemplates
{
    public sealed class SoNotProcessedEmail
    {
        public int QrPublicId { get; set; }
        public string ViewSoEmailUrl { get; set; }
    }
}
