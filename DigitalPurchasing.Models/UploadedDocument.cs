using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Models
{
    public class UploadedDocument : BaseModelWithOwner
    {
        public string Data { get; set; }

        public Guid UploadedDocumentHeadersId { get; set; }
        public UploadedDocumentHeaders Headers { get; set; }
    }

    public class UploadedDocumentHeaders
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Code { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }
        public string Qty { get; set; }
        public string Price { get; set; }
    }
}
