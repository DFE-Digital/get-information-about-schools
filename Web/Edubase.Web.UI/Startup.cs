using Owin;
using System.Configuration;

namespace Edubase.Web.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            if (ConfigurationManager.AppSettings["LoginProviderName"] == "SA") ConfigureSecureAccessAuth(app);
            else if (ConfigurationManager.AppSettings["LoginProviderName"] == "SASimulator") ConfigureSASimulatorAuth(app);
        }
    }
}
