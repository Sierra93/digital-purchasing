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
    }

    public class NomenclatureComparisonDistance
    {
        public string ComparisonName1 { get; set; }
        public string ComparisonName2 { get; set; }
        public double CompleteDistance =>
            (NameDistance + DigitsDistance - 2 * Math.Max(NamesLongestSubstringLen, NamesIntersect)) / (2 * Math.Max(ComparisonName1.Length, ComparisonName2.Length)) + (double) QtyDiff;
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
