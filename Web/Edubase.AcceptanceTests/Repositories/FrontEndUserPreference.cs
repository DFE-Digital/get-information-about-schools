namespace Edubase.AcceptanceTests.Repositories
{
    public sealed class FrontEndUserPreference
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public string? SavedSearchToken { get; set; }
    }
}
