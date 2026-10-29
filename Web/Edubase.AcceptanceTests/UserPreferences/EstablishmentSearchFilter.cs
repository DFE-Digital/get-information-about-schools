namespace Edubase.AcceptanceTests.UserPreferences
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
    }

    public sealed class AgeRangeFilter
    {
        public int? From { get; init; }
        public int? To { get; init; }
    }
}
