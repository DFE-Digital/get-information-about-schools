using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.Config
{
    public class AppConfigFeatureFlagProvider : IFeatureFlagProvider
    {
        public bool IsEnabled(string featureName)
            => bool.TryParse(ConfigurationManager.AppSettings[featureName], out var val) && val;
    }
}

/*
 * probably need to replace this in .net 8 with this
public class JsonFeatureFlagProvider : IFeatureFlagProvider
{
    private readonly IConfiguration _config;

    public JsonFeatureFlagProvider(IConfiguration config)
    {
        _config = config;
    }

    public bool IsEnabled(string featureName)
        => _config.GetValue<bool>($"FeatureFlags:{featureName}");
}
*/
