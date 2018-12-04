using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Models
{
    public class Analysis : BaseModelWithOwner
    {
        public CompetitionList CompetitionList { get; set; }
        public Guid CompetitionListId { get; set; }

        public ICollection<AnalysisVariant> Variants { get; set; }
    }
}
