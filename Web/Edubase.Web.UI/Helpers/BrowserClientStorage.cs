using Edubase.Common;
using Edubase.Services.Core;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Helpers
{
    public class BrowserClientStorage : IClientStorage
    {
        private HttpContextBase _httpContext;

        public BrowserClientStorage(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public string IPAddress { get => _httpContext.Request.UserHostAddress; }

        public string Get(string key) => _httpContext.Request.Cookies.AllKeys.Contains(key) ? _httpContext.Request.Cookies.Get(key)?.Value.Clean() : null;

        public string Save(string key, string value)
        {
            _httpContext.Response.Cookies.Set(new HttpCookie(key, value));
            return value;
        }
    }
}