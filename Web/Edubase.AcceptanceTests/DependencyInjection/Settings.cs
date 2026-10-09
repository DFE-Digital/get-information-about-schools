using System.Xml;
using System.Xml.Linq;

namespace Edubase.AcceptanceTests.DependencyInjection
{
    public class Settings
    {
        public string? GiasFrontEndBaseAddress { get; set; }
        public string? Environment { get; set; }
        public string? DataConnectionString { get; set; }
        public string? Redis { get; set; }
        public string? CompaniesHouseApiKey { get; set; }
        public string? GoogleApiKeyClientSide { get; set; }
        public string? GoogleTagManagerKey { get; set; }
        public string? AzureMapsApiKey { get; set; }
        public string? HttpAuthModuleCredentials { get; set; }
        public string? OSPlacesApiKey { get; set; }
        public string? CscpUsername { get; set; }
        public string? CscpPassword { get; set; }
        public string? SASimulatorGuid { get; set; }
        public string? GetGroupByIdFAKey { get; set; }
        public string? TexunaApiBaseAddress { get; set; }
        public string? ApiPassword { get; set; }
        public string? LookupApiBaseAddress { get; set; }
        public string? LookupApiPassword { get; set; }
        public string? SASimulatorUri { get; set; }
        public bool UseLocalSignIn { get; set; }

        internal static Settings Load()
        {
            const string fileName = "devsecrets.gias.config.alwaysignore";
            // Locate the repository configuration without copying secrets into build output.
            for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
            {
                var path = Path.Combine(directory.FullName, fileName);
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
                var value = (string?) entry.Attribute("connectionString");
                switch ((string?) entry.Attribute("name") ?? entry.Name.LocalName)
                {
                    case "DataConnectionString": settings.DataConnectionString = value; break;
                    case "Redis": settings.Redis = value; break;
                }
            }

            // Apply entries in order so the last duplicate key wins.
            foreach (var entry in configuration.Element("appSettings")?.Elements("add") ?? Enumerable.Empty<XElement>())
            {
                var value = (string?) entry.Attribute("value");
                switch ((string?) entry.Attribute("key"))
                {
                    case "GiasFrontEndBaseAddress": settings.GiasFrontEndBaseAddress = value; break;
                    case "Environment": settings.Environment = value; break;
                    case "CompaniesHouseApiKey": settings.CompaniesHouseApiKey = value; break;
                    case "GoogleApiKeyClientSide": settings.GoogleApiKeyClientSide = value; break;
                    case "GoogleTagManagerKey": settings.GoogleTagManagerKey = value; break;
                    case "AzureMapsApiKey": settings.AzureMapsApiKey = value; break;
                    case "HttpAuthModule.Credentials": settings.HttpAuthModuleCredentials = value; break;
                    case "OSPlacesApiKey": settings.OSPlacesApiKey = value; break;
                    case "CscpUsername": settings.CscpUsername = value; break;
                    case "CscpPassword": settings.CscpPassword = value; break;
                    case "SASimulatorGuid": settings.SASimulatorGuid = value; break;
                    case "GetGroupByIdFAKey": settings.GetGroupByIdFAKey = value; break;
                    case "TexunaApiBaseAddress": settings.TexunaApiBaseAddress = value; break;
                    case "api:Password": settings.ApiPassword = value; break;
                    case "LookupApiBaseAddress": settings.LookupApiBaseAddress = value; break;
                    case "LookupApiPassword": settings.LookupApiPassword = value; break;
                    case "SASimulatorUri": settings.SASimulatorUri = value; break;
                    case "UseLocalSignIn":
                        settings.UseLocalSignIn = bool.Parse(value);
                        break;
                }
            }

            return settings;
        }

    }
}
