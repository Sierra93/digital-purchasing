using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using F23.StringSimilarity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DigitalPurchasing.Services
{
    public sealed class NomenclatureComparisonService : INomenclatureComparisonService
    {
        public NomenclatureComparisonTerms CalculateComparisonTerms(string nomName)
        {
            var word2synonyms = new Dictionary<string, IReadOnlyList<string>>()
                {
                    { "очиститель", new List<string>() { "промывка" } }
                };

            Func<string, string> cleanupNomName = (str) => Regex.Replace(str, @"[^a-zA-Z\p{IsCyrillic}\s]", " ");
            Func<string, string> leaveOnlyDigits = (str) => Regex.Replace(str, "[^0-9]", " ").ReplaceSpacesWithOneSpace();
            Func<string, string> onlyDigitsOrderedByGroupLen = (str) => leaveOnlyDigits(str).Split(' ').OrderBy(s => s.Length).JoinNotEmpty(" ");
            Func<string, string> orderWords = (str) => string.Join(' ', str.Split(' ').OrderBy(w => w));
            Func<string, string> removeNoize = (str) => string.Join(' ', str.Split(' ').Where(w => w.Length > 2));
            Func<string, string> replaceSynonyms = (str) =>
            {
                var result = new List<string>();
                foreach (var word in str.Split(' '))
                {
                    foreach (var w2s in word2synonyms)
                    {
                        result.Add(w2s.Value.Any(s => s.Equals(word, StringComparison.InvariantCultureIgnoreCase)) ? w2s.Key : word);
                    }
                }
                return string.Join(" ", result);
            };
            Func<string, string> getDimensions = (str) =>
            {
                var match = Regex.Match(str, @"\s?\d+[,.]?\d*\s?[x*х]\s?\d+[,.]?\d*\s?([x*х]\s?\d+[,.]?\d*)?", RegexOptions.IgnoreCase);
                var dims = match.Success ? match.Groups[0].Value.RemoveSpaces() : string.Empty;
                var cleanedUp = Regex.Replace(dims.Replace('.', ','), @"[^\d\*,]", "*", RegexOptions.IgnoreCase);
                return cleanedUp;
            };
            Func<string, string, string, string, (string nameWithDims1, string nameWithDims2)> getNamesWithDims = (name1, dims1, name2, dims2) =>
            {
                if (!string.IsNullOrEmpty(dims1) && !string.IsNullOrEmpty(dims2))
                {
                    return ($"{name1} {dims1}", $"{name2} {dims2}");
                }

                return (name1, name2);
            };

            var dimensions = getDimensions(nomName);
            var terms = new NomenclatureComparisonTerms()
            {
                AdjustedName = orderWords(replaceSynonyms(removeNoize(cleanupNomName(nomName).ReplaceSpacesWithOneSpace()).Trim().ToLower())),
                NomDimensions = string.IsNullOrWhiteSpace(dimensions) ? null : dimensions,
                AdjustedDigits = onlyDigitsOrderedByGroupLen(nomName)
            };

            return terms;
        }

        public NomenclatureComparisonDistance CalculateDistance(NomenclatureComparisonTerms nom1, NomenclatureComparisonTerms nom2,
            bool isSameUoms, decimal nomQty1, decimal nomQty2)
        {
            var names = nom1.NomDimensions == null || nom2.NomDimensions == null
                            ? (nom1.AdjustedName, nom2.AdjustedName)
                            : (nom1.AdjustedNameWithDimensions, nom2.AdjustedNameWithDimensions);

            var alg = new Levenshtein();
            var maxNameLen = Math.Max(names.Item1.Length, names.Item2.Length);

            var distance = new NomenclatureComparisonDistance
            {
                ComparisonName1 = names.Item1,
                ComparisonName2 = names.Item2
            };

            distance.NamesIntersect = string.Join("", distance.ComparisonName1.Split(' ').Intersect(distance.ComparisonName2.Split(' '))).Length;
            distance.NamesLongestSubstringLen = LongestCommonSubstring(distance.ComparisonName1.RemoveSpaces(), distance.ComparisonName2.RemoveSpaces());
            distance.NameDistance = alg.Distance(distance.ComparisonName1, distance.ComparisonName2);
            distance.DigitsDistance = alg.Distance(nom1.AdjustedDigits, nom2.AdjustedDigits);
            distance.QtyDiff = isSameUoms ? Math.Abs(nomQty2 - nomQty1) / (10 * Math.Max(nomQty2, nomQty1)) : 0.1m;

            return distance;
        }

        public NomenclatureComparisonDistance CalculateDistance(NomenclatureComparisonTerms nom1, NomenclatureComparisonTerms nom2) =>
            CalculateDistance(nom1, nom2, false, 1, 1);

        private static int LongestCommonSubstring(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
                return 0;

            var num = new int[str1.Length, str2.Length];
            int maxlen = 0;

            for (int i = 0; i < str1.Length; i++)
            {
                for (int j = 0; j < str2.Length; j++)
                {
                    if (str1[i] != str2[j])
                        num[i, j] = 0;
                    else
                    {
                        if ((i == 0) || (j == 0))
                            num[i, j] = 1;
                        else
                            num[i, j] = 1 + num[i - 1, j - 1];

                        if (num[i, j] > maxlen)
                        {
                            maxlen = num[i, j];
                        }
                    }
                }
            }
            return maxlen;
        }
    }
}
