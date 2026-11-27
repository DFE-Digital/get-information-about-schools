namespace Edubase.AcceptanceTests.Users
{
    public class UsersFromHardCodedValues : IUsers
    {
        private readonly string environment;

        public UsersFromHardCodedValues(string environment)
        {
            this.environment = environment;
        }

        public User GetBackOfficeUser()
        {
            if (environment == "test")
            {
                return new User
                {
                    NameId = "user_20170322155311_3600264",
                    AttributeStatementValue = "3600264",
                    Comment = "GIAS Backoffice - TESTER ONLY"
                };
            }

            return new User
            {
                NameId = "user_20170322155311_3600154",
                AttributeStatementValue = "3600154",
                Comment = "GIAS Backoffice - TESTER ONLY"
            };
        }
    }
}
