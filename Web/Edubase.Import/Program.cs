using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            var copier = new SqlBulkCopy(ConfigurationManager.AppSettings["sql"]);



        }
    }
}
