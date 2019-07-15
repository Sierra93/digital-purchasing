using DigitalPurchasing.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Web.ViewModels.Analysis
{
    public class AnalysisIndexVm
    {
        public Guid ClId { get; set; }
        public IEnumerable<SSReportSimple> Reports { get; set; }
    }
}
