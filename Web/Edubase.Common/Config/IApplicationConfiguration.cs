using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.Config
{
    public interface IApplicationConfiguration
    {
        string Get(string name);
        string GetConnectionString(string name);
    }
}
