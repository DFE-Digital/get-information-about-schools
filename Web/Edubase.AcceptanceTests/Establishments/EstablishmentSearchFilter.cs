using System.Globalization;
using static System.Net.WebRequestMethods;

namespace Edubase.AcceptanceTests.Establishments
{
    public sealed class EstablishmentSearchFilter
    {
        public string SearchType { get; init; } = "EstablishmentAll";
        public string? FilterSet { get; init; }
        public string? SavedFilterToken { get; init; }

        public string? TextSearchText { get; init; }
        public string? TextSearchAutoSuggestValue { get; init; }
        public string? LocationSearchText { get; init; }
        public string? LocationSearchAutoSuggestValue { get; init; }

        public string? SortBy { get; init; }
        public double? RadiusInMiles { get; init; }

        public IReadOnlyCollection<int> EstablishmentTypeIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> StatusIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> EducationPhaseIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> LocalAuthorityIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> ReligiousCharacterIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> AdmissionsPolicyIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> BoardingProvisionIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> DioceseIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> DistrictIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> FurtherEducationTypeIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> GenderIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> GovernmentOfficeRegionIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> NurseryProvisionIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> ParliamentaryConstituencyIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> RegistrationStatusIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> ReligiousEthosIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> RscRegionIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> Section41Ids { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> SixthFormProvisionIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> SpecialClassesProvisionIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> TypeOfSenProvisionIds { get; init; } = Array.Empty<int>();
        public IReadOnlyCollection<int> UrbanRuralIds { get; init; } = Array.Empty<int>();

        public DateTime? OpenDateFrom { get; init; }
        public DateTime? OpenDateTo { get; init; }
        public DateTime? CloseDateFrom { get; init; }
        public DateTime? CloseDateTo { get; init; }
        public AgeRangeFilter? AgeRangeLow { get; init; }
        public AgeRangeFilter? AgeRangeHigh { get; init; }

        internal FormUrlEncodedContent AsFormUrlEncodedContent()
        {
            var fields = new List<KeyValuePair<string, string>>();

            AddField(fields, "SearchType", this.SearchType);
            AddField(fields, "filter-set", this.FilterSet);
            AddField(fields, "SavedFilterToken", this.SavedFilterToken);

            if (this.SearchType == "Text")
            {
                AddField(fields, "TextSearchModel.Text", this.TextSearchText);
                AddField(fields, "TextSearchModel.AutoSuggestValue", this.TextSearchAutoSuggestValue);
            }

            if (this.SearchType == "Location")
            {
                AddField(fields, "LocationSearchModel.Text", this.LocationSearchText);
                AddField(fields, "LocationSearchModel.AutoSuggestValue", this.LocationSearchAutoSuggestValue);
                fields.Add(new KeyValuePair<string, string>(
                    "aa",
                    (this.RadiusInMiles ?? 3).ToString("0.##", CultureInfo.InvariantCulture)));
            }

            var sortBy = this.SortBy ?? (this.SearchType == "Location" ? "d" : "a");
            AddField(fields, "z", sortBy);

            AddValues(fields, "a", this.EstablishmentTypeIds);
            AddValues(fields, "b", this.StatusIds);
            AddValues(fields, "c", this.EducationPhaseIds);
            AddValues(fields, "d", this.LocalAuthorityIds);
            AddValues(fields, "e", this.ReligiousCharacterIds);
            AddValues(fields, "g", this.AdmissionsPolicyIds);
            AddValues(fields, "h", this.BoardingProvisionIds);
            AddValues(fields, "i", this.DioceseIds);
            AddValues(fields, "j", this.DistrictIds);
            AddValues(fields, "k", this.FurtherEducationTypeIds);
            AddValues(fields, "l", this.GenderIds);
            AddValues(fields, "m", this.GovernmentOfficeRegionIds);
            AddValues(fields, "n", this.NurseryProvisionIds);
            AddValues(fields, "p", this.ParliamentaryConstituencyIds);
            AddValues(fields, "8", this.RegistrationStatusIds);
            AddValues(fields, "q", this.ReligiousEthosIds);
            AddValues(fields, "r", this.RscRegionIds);
            AddValues(fields, "s", this.Section41Ids);
            AddValues(fields, "t", this.SixthFormProvisionIds);
            AddValues(fields, "u", this.SpecialClassesProvisionIds);
            AddValues(fields, "v", this.TypeOfSenProvisionIds);
            AddValues(fields, "w", this.UrbanRuralIds);

            AddDate(fields, "OpenDateFrom", this.OpenDateFrom);
            AddDate(fields, "OpenDateTo", this.OpenDateTo);
            AddDate(fields, "CloseDateFrom", this.CloseDateFrom);
            AddDate(fields, "CloseDateTo", this.CloseDateTo);
            AddAgeRange(fields, "AgeRangeLow", this.AgeRangeLow);
            AddAgeRange(fields, "AgeRangeHigh", this.AgeRangeHigh);

            var sortedFields = fields
                .Distinct()
                .OrderBy(field => $"{field.Key}={field.Value}", StringComparer.Ordinal);

            var content = new FormUrlEncodedContent(sortedFields);

            return content;
        }

        private static void AddField(ICollection<KeyValuePair<string, string>> fields, string name, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                fields.Add(new KeyValuePair<string, string>(name, value));
            }
        }

        private static void AddValues(ICollection<KeyValuePair<string, string>> fields, string name, IEnumerable<int> values)
        {
            foreach (var value in values)
            {
                fields.Add(new KeyValuePair<string, string>(name, value.ToString(CultureInfo.InvariantCulture)));
            }
        }

        private static void AddDate(ICollection<KeyValuePair<string, string>> fields, string name, DateTime? value)
        {
            if (!value.HasValue)
            {
                return;
            }

            var date = value.Value;
            fields.Add(new KeyValuePair<string, string>($"{name}.Day", date.Day.ToString("00", CultureInfo.InvariantCulture)));
            fields.Add(new KeyValuePair<string, string>($"{name}.Month", date.Month.ToString("00", CultureInfo.InvariantCulture)));
            fields.Add(new KeyValuePair<string, string>($"{name}.Year", date.Year.ToString("0000", CultureInfo.InvariantCulture)));
        }

        private static void AddAgeRange(ICollection<KeyValuePair<string, string>> fields, string name, AgeRangeFilter? value)
        {
            if (value?.From is int from)
            {
                fields.Add(new KeyValuePair<string, string>($"{name}.From", from.ToString(CultureInfo.InvariantCulture)));
            }

            if (value?.To is int to)
            {
                fields.Add(new KeyValuePair<string, string>($"{name}.To", to.ToString(CultureInfo.InvariantCulture)));
            }
        }
    }

    public sealed class AgeRangeFilter
    {
        public int? From { get; init; }
        public int? To { get; init; }
    }
}
