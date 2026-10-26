using Edubase.Common.Config;
using Microsoft.Extensions.Configuration;

namespace Edubase.Web.UI.Helpers;

public class ConfigurationFeatureFlagProvider : IFeatureFlagProvider
{
    private readonly IConfiguration _configuration;

    public ConfigurationFeatureFlagProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsEnabled(string featureName) => bool.TryParse(_configuration[featureName], out var enabled) && enabled;
}
