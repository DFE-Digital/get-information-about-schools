using System;
using System.Configuration;

namespace Edubase.Web.UI
{
    public class StartupSASimulator
    {
        public static Uri PublicOrigin => Uri.TryCreate(
            ConfigurationManager.AppSettings[nameof(PublicOrigin)],
            UriKind.Absolute,
            out var uri)
            ? uri
            : null;
    }
}
