namespace Edubase.AcceptanceTests.Establishments
{
    public sealed class EstablishmentSearchResults
    {
        public IReadOnlyList<EstablishmentSearchResult> Establishments { get; init; } = Array.Empty<EstablishmentSearchResult>();
    }

    public sealed class EstablishmentSearchResult
    {
        public int? Urn { get; set; }
    }
}
