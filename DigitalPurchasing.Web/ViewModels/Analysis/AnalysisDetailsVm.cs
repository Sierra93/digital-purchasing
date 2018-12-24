using System;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Web.ViewModels.Analysis
{
    public class AnalysisDetailsVm
    {
        public Guid ClId { get; set; }

        public AnalysisDetails Data { get; set; }
    }
}
