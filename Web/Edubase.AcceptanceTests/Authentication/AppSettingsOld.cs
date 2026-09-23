using Microsoft.Extensions.Configuration;

namespace Edubase.AcceptanceTests.Authentication
{
    public sealed class AppSettingsOld
    {
        private readonly IConfiguration _configuration;

        public AppSettingsOld()
        {
            // Load testSettings.json to get the environment
            var envConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testSettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Default to test if no environment value set in testSettings.json.
            var environment = envConfig.GetValue<string>("test:environment") ?? "test";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"signinSimUsers.{environment}.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<AppSettings>()
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            // Get secrets for the respective env and set as vars
            var username = _configuration[$"{environment}:web:username"];
            var password = _configuration[$"{environment}:web:password"];
            var authUsername = _configuration[$"{environment}:api:authUsername"];
            var authPassword = _configuration[$"{environment}:api:authPassword"];

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)
                && !string.IsNullOrEmpty(authUsername) && !string.IsNullOrEmpty(authPassword))
            {
                // Use ConfigurationRoot's indexer to override values
                ((IConfigurationRoot) _configuration)["web:username"] = username;
                ((IConfigurationRoot) _configuration)["web:password"] = password;
                ((IConfigurationRoot) _configuration)["api:authUsername"] = authUsername;
                ((IConfigurationRoot) _configuration)["api:authPassword"] = authPassword;
            }

            var baseUri = _configuration.GetSection("web")?.GetValue<string>("domain");
        }

        public ApiOptions ApiConfig()
        {
            var section = _configuration.GetRequiredSection(ApiOptions.Key);
            var config = section.Get<ApiOptions>();
            if (config == null)
                throw new InvalidOperationException($"Configuration section '{ApiOptions.Key}' is missing or invalid.");
            return config;
        }

        public BackendOptions BackendConfig()
        {
            var section = _configuration.GetRequiredSection(BackendOptions.Key);
            var config = section.Get<BackendOptions>();
            if (config == null)
                throw new InvalidOperationException($"Configuration section '{BackendOptions.Key}' is missing or invalid.");
            return config;
        }

        public DatabaseOptions DatabaseConfig()
        {
            var section = _configuration.GetRequiredSection(DatabaseOptions.Key);
            var config = section.Get<DatabaseOptions>();
            if (config == null)
                throw new InvalidOperationException($"Configuration section '{DatabaseOptions.Key}' is missing or invalid.");
            return config;
        }

        public UserOptions UserConfig()
        {
            var users = _configuration.GetSection("users").Get<Dictionary<string, UserCredentials>>();

            if (users == null || !users.Any())
                throw new InvalidOperationException("No users loaded from configuration. Check JSON structure and environment.");

            Console.WriteLine($"[AppSettings] Loaded users: {users.Count}");

            return new UserOptions { Users = users };
        }

        public WebOptions WebConfig()
        {
            var section = _configuration.GetRequiredSection(WebOptions.Key);
            var config = section.Get<WebOptions>();
            if (config == null)
                throw new InvalidOperationException($"Configuration section '{WebOptions.Key}' is missing or invalid.");
            return config;
        }

        public TestOptions TestConfig()
        {
            var config = _configuration.GetRequiredSection(TestOptions.Key).Get<TestOptions>();
            Console.WriteLine($"[TestConfig] Environment: {config.Environment}");
            return config;
        }
    }
}
