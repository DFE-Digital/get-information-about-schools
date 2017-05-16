using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI
{
    using System.IO;
    using System.Web.Hosting;
    using static ConfigurationManager;
    public static class AuthConfig
    {
        public static TimeSpan SessionExpireTimeSpan
        {
            get
            {
                var configuredValue = AppSettings["SessionExpireTimeSpan"];
                return configuredValue != null
                    ? TimeSpan.Parse(configuredValue)
                    : TimeSpan.FromMinutes(60);
            }
        }

        public static string ApplicationIdpEntityId => AppSettings[nameof(ApplicationIdpEntityId)];
        public static Uri ExternalAuthDefaultCallbackUrl => new Uri(AppSettings[nameof(ExternalAuthDefaultCallbackUrl)]);
        public static Uri ExternalIdpEntityId => new Uri(AppSettings[nameof(ExternalIdpEntityId)]);
        public static string ExternalIdpLogoutUri => AppSettings[nameof(ExternalIdpLogoutUri)];

        public static string ExternalIdpMetadataPath
        {
            get
            {
                if (ExternalIdpEntityId.Host == "stubidp.kentor.se")
                {
                    return ExternalIdpEntityId.ToString();
                }

                var filenamePrefix = HostingEnvironment.MapPath($"~/App_Data/{ExternalIdpEntityId.Host}-metadata");
                var fullPath = $"{filenamePrefix}.xml";
//#if DEBUG
//                var debugFullPath = $"{filenamePrefix}-DEBUG.xml";
//                if (File.Exists(debugFullPath))
//                {
//                    fullPath = debugFullPath;
//                }
//#endif

                return fullPath;
            }
        }

        public static string ExternalIdpCertificatePath => HostingEnvironment.MapPath($"~/App_Data/{ExternalIdpEntityId.Host}-certificate.cer");
    }
}