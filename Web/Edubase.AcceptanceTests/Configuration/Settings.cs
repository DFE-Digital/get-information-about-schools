using System.Xml;
using System.Xml.Linq;

namespace Edubase.AcceptanceTests.Configuration
{

    internal class Settings
    {
        internal string Environment { get; set; }
        internal bool UseLocalSignIn { get; set; }

        internal ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        internal GiasFrontEnd GiasFrontEnd { get; set; } = new GiasFrontEnd();
        internal FeatureFlags FeatureFlags { get; set; } = new FeatureFlags();

        private Settings()
        {
        }

        internal static Settings CreateFromXml(string fileName)
        {
            for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
            {
                var path = Path.Join(directory.FullName, fileName);

                if (!File.Exists(path)) continue;

                using var reader = XmlReader.Create(path, new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                });

                return DeserializeSettings(reader);
            }

            throw new FileNotFoundException(
                $"Acceptance tests require '{fileName}' in the repository root (above the test output directory).", fileName);
        }

        internal static Settings DeserializeSettings(XmlReader reader)
        {
            var configuration = XDocument.Load(reader).Root;
            if (configuration?.Name != "configuration")
                throw new InvalidOperationException("Expected a configuration XML element.");

            var settings = new Settings();
            foreach (var entry in configuration.Element("connectionStrings")?.Elements() ?? Enumerable.Empty<XElement>())
            {
                var value = (string) entry.Attribute("connectionString");
                switch ((string) entry.Attribute("name") ?? entry.Name.LocalName)
                {
                    case "AzureTableStorage": settings.ConnectionStrings.AzureTableStorage = value; break;
                    case "Redis": settings.ConnectionStrings.Redis = value; break;
                    case "AzureSqlServer": settings.ConnectionStrings.AzureSqlServer = value; break;
                }
            }

            // Apply entries in order so the last duplicate key wins.
            foreach (var entry in configuration.Element("appSettings")?.Elements("add") ?? Enumerable.Empty<XElement>())
            {
                var value = (string) entry.Attribute("value");
                switch ((string) entry.Attribute("key"))
                {
                    case "GiasFrontEndBaseAddress": settings.GiasFrontEnd.BaseAddress = value; break;
                    case "Environment": settings.Environment = value; break;
                    case "HttpAuthModule.Credentials": settings.GiasFrontEnd.HttpAuthModuleCredentials = value; break;
                    case "UseLocalSignIn": settings.UseLocalSignIn = bool.Parse(value); break;
                    case "Feature_UserPreferencesMigration": settings.FeatureFlags.UserPreferencesMigrated = bool.Parse(value); break;
                }
            }

            return settings;
        }
    }
}
