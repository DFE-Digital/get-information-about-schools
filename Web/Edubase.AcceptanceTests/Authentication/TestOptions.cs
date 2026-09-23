namespace Edubase.AcceptanceTests.Authentication
{
    public sealed class TestOptions
    {
        public const string Key = "test";
        public string? Environment { get; set; }
        public bool SeedData { get; set; }
        public int RateLimiting { get; set; }
    }
}
