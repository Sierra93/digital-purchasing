using System;

namespace DigitalPurchasing.Models
{
    public class AnalysisVariant : BaseModel
    {
        public Analysis Analysis { get; set; }
        public Guid AnalysisId { get; set; }
    }
}
