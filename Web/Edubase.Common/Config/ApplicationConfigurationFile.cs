using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.Config
{
    public class ApplicationConfigurationFile : IApplicationConfiguration
    {
        public string Get(string name) => ConfigurationManager.AppSettings[name];

        public string GetConnectionString(string name) => ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
    }
}
