using Edubase.Common;
using Edubase.Services.Core;
using Microsoft.AspNetCore.Http;

namespace Edubase.Web.UI.Helpers
{
    public class BrowserClientStorage : IClientStorage
    {
        private readonly HttpContext _httpContext;

        public BrowserClientStorage(IHttpContextAccessor accessor)
        {
            _httpContext = accessor.HttpContext;
        }

        public string IPAddress => _httpContext?.Connection?.RemoteIpAddress?.ToString();

        public string Get(string key)
        {
            return _httpContext?.Request?.Cookies?.ContainsKey(key) == true ? (_httpContext.Request.Cookies[key]?.Clean()) :  null;
        }

        public string Save(string key, string value)
        {
            var options = new CookieOptions
            {
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                Secure = true
            };

            _httpContext?.Response?.Cookies?.Append(key, value, options);
            return value;
        }
    }
}
