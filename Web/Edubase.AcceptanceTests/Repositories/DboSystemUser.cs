namespace Edubase.AcceptanceTests.Repositories
{
    public class DboSystemUser
    {
        public string username { get; set; }
        public decimal saUserId { get; set; }
        public byte wsEnabled { get; set; }
        public byte enabled { get; set; }
        public string UserGroupCode { get; set; }
        public long? URN { get; set; }
        public long? UID { get; set; }
        public string? LocalAuthority { get; set; }
    }
}
