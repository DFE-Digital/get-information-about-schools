using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.Config
{
    public class DictionaryApplicationConfiguration : Dictionary<string, string>, IApplicationConfiguration
    {
        private string _guid = "2cfbeae7-072e-4953-b5b1-2f84749db589";

        string IApplicationConfiguration.Get(string name)
        {
            return this.Get(name);
        }

        string IApplicationConfiguration.GetConnectionString(string name)
        {
            return this.Get(string.Concat(_guid, "-" + name));
        }

        public void SetConnectionString(string name, string value)
        {
            this[string.Concat(_guid, "-" + name)] = value;
        }
    }
}
