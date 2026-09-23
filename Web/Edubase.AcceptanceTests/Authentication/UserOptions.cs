namespace Edubase.AcceptanceTests.Authentication
{
    public class UserOptions
    {
        public const string Key = "users";
        public Dictionary<string, UserCredentials> Users { get; set; }
    }

    public class UserCredentials
    {
        public string NameId { get; set; }
        public string AttributeStatementValue { get; set; }
        public string Comment { get; set; } // Comments for info
    }
}
