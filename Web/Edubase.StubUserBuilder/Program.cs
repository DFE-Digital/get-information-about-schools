using Edubase.Data.Entity.Lookups;
using Edubase.Services.Enums;
using Edubase.Services.Security;
using Edubase.Services.Security.Permissions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.StubUserBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Configurator().Configure();
            var json = JsonConvert.SerializeObject(config, new JsonSerializerSettings { Formatting = Formatting.Indented });
            Console.Write(json);
            File.WriteAllText("stubidp_config.json", json);
            Console.ReadKey();
        }

    }
}
