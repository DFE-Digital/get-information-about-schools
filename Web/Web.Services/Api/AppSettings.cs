using System.Configuration;

namespace Web.Services.Api
{
    public class AppSettings : IAppSettings
    {
        public string ApiBaseUrl => ConfigurationManager.AppSettings["ApiBaseUrl"];
        public string ApiUserName => ConfigurationManager.AppSettings["ApiUserName"];
        public string ApiPassword => ConfigurationManager.AppSettings["ApiPassword"];
    }
}