using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces.Analysis
{
    public interface IAnalysisCore
    {
        List<AnalysisResult> Run(params AnalysisVariantData[] variantDatas);
        AnalysisResult Run(AnalysisVariantData variantData);
    }
}
