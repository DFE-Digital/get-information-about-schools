using System;
using System.Configuration;

namespace Edubase.Web.UI
{
    public partial class StartupSecureAccess
    {
        public static TimeSpan ConfiguredExpireTimeSpan => TimeSpan.TryParse(ConfigurationManager.AppSettings["SessionExpireTimeSpan"], out var value)
            ? value
            : TimeSpan.FromHours(8);

        public static string ApplicationIdpEntityId => ConfigurationManager.AppSettings[nameof(ApplicationIdpEntityId)] ?? string.Empty;

        public static Uri ExternalAuthDefaultCallbackUrl => Uri.TryCreate(
            ConfigurationManager.AppSettings[nameof(ExternalAuthDefaultCallbackUrl)],
            UriKind.Absolute,
            out var uri)
            ? uri
            : new Uri("https://localhost/");

        public static Uri MetadataLocation => Uri.TryCreate(
            ConfigurationManager.AppSettings[nameof(MetadataLocation)],
            UriKind.Absolute,
            out var uri)
            ? uri
            : new Uri("https://localhost/");

        public static Uri PublicOrigin => Uri.TryCreate(
            ConfigurationManager.AppSettings[nameof(PublicOrigin)],
            UriKind.Absolute,
            out var uri)
            ? uri
            : null;
    }
}
