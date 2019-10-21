using System;

namespace DigitalPurchasing.Models
{
    public class DefaultUom : BaseModelWithOwner
    {
        public Guid PackagingUomId { get; set; }

        public Guid MassUomId { get; set; }
        
        public Guid ResourceUomId { get; set; }
        public Guid ResourceBatchUomId { get; set; }
    }
}
