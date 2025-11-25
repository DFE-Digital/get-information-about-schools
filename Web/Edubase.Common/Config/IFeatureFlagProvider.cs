using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.Config
{
    public interface IFeatureFlagProvider
    {
        bool IsEnabled(string featureName);
    }
}
