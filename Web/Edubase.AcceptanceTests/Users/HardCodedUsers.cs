namespace Edubase.AcceptanceTests.Users
{
    public class HardCodedUsers : IUsers
    {
        public User GetBackOfficeUser()
        {
            return new User
            {
                NameId = "user_20170322155311_3600154",
                AttributeStatementValue = "3600154",
                Comment = "GIAS Backoffice - TESTER ONLY"
            };
        }
    }
}
