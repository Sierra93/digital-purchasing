using System.Collections.Generic;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Web.ViewModels.CompetitionList
{
    public class CompetitionListEditVm
    {
        public CompetitionListVm CompetitionList { get; set; }
        public List<SSReportSimple> Reports { get; set; }
    }
}
