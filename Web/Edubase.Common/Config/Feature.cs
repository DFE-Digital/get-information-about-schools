using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.Config
{
    public static class Feature
    {
        public static IFeatureFlagProvider Provider { get; set; }

        public static bool IsEnabled(string feature)
            => Provider?.IsEnabled(feature) ?? false;
    }
}
