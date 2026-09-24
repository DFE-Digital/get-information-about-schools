namespace Edubase.AcceptanceTests.Authentication
{
    public sealed class AppSettings
    {
        public TestOptions TestConfig()
        {
            return new TestOptions
            {
                Environment = "dev"
            };
        }

        public UserOptions UserConfig()
        {
            var userCredentials = new UserCredentials
            {
                NameId = "user_20170322155311_3600154",
                AttributeStatementValue = "3600154",
                Comment = "GIAS Backoffice - TESTER ONLY"
            };

            return new UserOptions
            {
                Users = new Dictionary<string, UserCredentials>
                {
                    { "backOfficeUserId", userCredentials }
                }
            };
        }

        public WebOptions WebConfig()
        {
            return new WebOptions
            {
                Scheme = "https",
                Domain = "localhost",
                Port = 44309,
                BasicAuthEnabled = false,
                Username = string.Empty,
                Password = string.Empty
            };
        }
    }
}
