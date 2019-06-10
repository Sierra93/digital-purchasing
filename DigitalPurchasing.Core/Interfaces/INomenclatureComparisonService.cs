using DigitalPurchasing.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface INomenclatureComparisonService
    {
        NomenclatureComparisonTerms CalculateComparisonTerms(string nomName);
        NomenclatureComparisonDistance CalculateDistance(NomenclatureComparisonTerms nom1, NomenclatureComparisonTerms nom2,
            bool isSameUoms, decimal nomQty1, decimal nomQty2);
        NomenclatureComparisonDistance CalculateDistance(NomenclatureComparisonTerms nom1, NomenclatureComparisonTerms nom2);
    }

    public class NomenclatureComparisonDistance
    {
        public string ComparisonName1 { get; set; }
        public string ComparisonName2 { get; set; }
        public string AdjustedDigits1 { get; set; }
        public string AdjustedDigits2 { get; set; }
        private int MaxComparisonNameLen => Math.Max(ComparisonName1.RemoveSpaces().Length, ComparisonName2.RemoveSpaces().Length);
        private double MaxSimilarChainLen => Math.Max((NamesLongestSubstringLen > 3 ? 2 : 1) * NamesLongestSubstringLen, 2.5 * NamesIntersect);
        private int MaxDigitsLen => Math.Max(AdjustedDigits1.RemoveSpaces().Length, AdjustedDigits2.RemoveSpaces().Length);
        public double CompleteDistance =>
            (NameDistance + DigitsDistance - MaxSimilarChainLen) / (2 * (MaxComparisonNameLen + MaxDigitsLen)) + (double) QtyDiff;
        public double NameDistance { get; set; }
        public decimal QtyDiff { get; set; }
        public int NamesIntersect { get; set; }
        public int NamesLongestSubstringLen { get; set; }
        public double DigitsDistance { get; set; }
    }

    public class NomenclatureComparisonTerms
    {
        public string AdjustedName { get; set; }
        public string NomDimensions { get; set; }
        public string AdjustedNameWithDimensions => $"{AdjustedName} {NomDimensions}";
        public string AdjustedDigits { get; set; }
    }
}
