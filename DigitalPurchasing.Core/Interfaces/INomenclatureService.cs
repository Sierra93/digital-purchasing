using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface INomenclatureService
    {
        NomenclatureIndexData GetData(int page, int perPage, string sortField, bool sortAsc, string search);
        NomenclatureDetailsData GetDetailsData(Guid nomId, int page, int perPage, string sortField, bool sortAsc, string sortBySearch);
        NomenclatureVm CreateOrUpdate(NomenclatureVm model);
        void CreateOrUpdate(List<NomenclatureVm> models, Guid ownerId);
        NomenclatureVm GetById(Guid id, bool globalSearch = false);
        IEnumerable<NomenclatureVm> GetByNames(params string[] nomenclatureNames);
        NomenclatureAutocompleteResult Autocomplete(AutocompleteOptions options);
        BaseResult<NomenclatureAutocompleteResult.AutocompleteResultItem> AutocompleteSingle(Guid id);
        void Delete(Guid id);
        NomenclatureWholeData GetWholeNomenclature();
        NomenclatureVm FindBestFuzzyMatch(Guid ownerId, string nomName, int maxNameDistance);
    }

    public class NomenclatureWholeData
    {
        public IDictionary<NomenclatureIndexDataItem, NomenclatureDetailsData> Nomenclatures { get; set; } =
            new Dictionary<NomenclatureIndexDataItem, NomenclatureDetailsData>();
    }

    public class NomenclatureSearchTerms
    {
        public string AdjustedName { get; private set; }
        public string NomDimensions { get; private set; }
        public string AdjustedNameWithDimensions => $"{AdjustedName} {NomDimensions}";
        public string AdjustedDigits { get; private set; }

        public NomenclatureSearchTerms(string nomName)
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

            AdjustedName = orderWords(replaceSynonyms(removeNoize(cleanupNomName(nomName).ReplaceSpacesWithOneSpace()).Trim().ToLower()));
            NomDimensions = getDimensions(nomName);
            AdjustedDigits = onlyDigitsOrderedByGroupLen(nomName);
        }
    }

    public class NomenclatureDetailsDataItem
    {
        public Guid Id { get; set; }

        public int ClientType { get; set; }
        public string ClientName { get; set; }
        public int? ClientPublicId { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public Guid BatchUomId { get; set; }
        public string BatchUomName { get; set; }

        public Guid MassUomId { get; set; }
        public string MassUomName { get; set; }
        public decimal MassUomValue { get; set; }
       
        public Guid ResourceUomId { get; set; }
        public string ResourceUomName { get; set; }
        public decimal ResourceUomValue { get; set; }

        public Guid ResourceBatchUomId { get; set; }
        public string ResourceBatchUomName { get; set; }

        public string PackUomName { get; set; }
        public decimal? PackUomValue { get; set; }
    }

    public class NomenclatureDetailsData : BaseDataResponse<NomenclatureDetailsDataItem>
    {
    }

    public class NomenclatureIndexDataItem
    {
        public Guid Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }
        public string NameEng { get; set; }

        public Guid BatchUomId { get; set; }
        public string BatchUomName { get; set; }

        public Guid MassUomId { get; set; }
        public string MassUomName { get; set; }
        public decimal MassUomValue { get; set; }
       
        public Guid ResourceUomId { get; set; }
        public string ResourceUomName { get; set; }
        public decimal ResourceUomValue { get; set; }

        public Guid ResourceBatchUomId { get; set; }
        public string ResourceBatchUomName { get; set; }

        public Guid? PackUomId { get; set; }
        public string PackUomName { get; set; }
        public decimal PackUomValue { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryFullName { get; set; }
        public bool HasAlternativeWithRequiredName { get; set; }
    }

    //todo: rename to NomenclatureDto
    public class NomenclatureVm
    {
        public Guid OwnerId { get; set; }

        public Guid Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }
        public string NameEng { get; set; }

        public Guid BatchUomId { get; set; }
        public string BatchUomName { get; set; }

        public Guid MassUomId { get; set; }
        public string MassUomName { get; set; }
        public decimal MassUomValue { get; set; }
       
        public Guid ResourceUomId { get; set; }
        public string ResourceUomName { get; set; }
        public decimal ResourceUomValue { get; set; }

        public Guid ResourceBatchUomId { get; set; }
        public string ResourceBatchUomName { get; set; }

        public Guid? PackUomId { get; set; }
        public string PackUomName { get; set; }
        public decimal PackUomValue { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryFullName { get; set; }
    }

    public class NomenclatureIndexData : BaseDataResponse<NomenclatureIndexDataItem>
    {
    }

    public class AutocompleteBaseOptions
    {
        public string Query { get; set; }
    }

    public class AutocompleteOptions : AutocompleteBaseOptions
    {
        public Guid ClientId { get; set; }
        public ClientType ClientType { get; set; }
        public bool SearchInAlts { get; set; } = false;
        public Guid OwnerId { get; set; }
    }

    public class NomenclatureAutocompleteResult
    {
        public class AutocompleteResultItem
        {
            public string Name { get; set; }
            public string NameEng { get; set; }
            public Guid Id { get; set; }
            public string Code { get; set; }
            public string BatchUomName { get; set; }
            public Guid BatchUomId { get; set; }
            public bool IsFullMatch { get; set; }
        }

        public List<AutocompleteResultItem> Items { get; set; } = new List<AutocompleteResultItem>();
    }
}
