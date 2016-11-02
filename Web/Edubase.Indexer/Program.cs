using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace Edubase.Indexer
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var host = new JobHost();
            // The following code will invoke a function called ManualTrigger and 
            // pass in data (value in this case) to the function
            host.Call(typeof(Functions).GetMethod(nameof(Functions.RebuildEstablishmentsIndex)), new { value = 20 });
        }
    }
}
