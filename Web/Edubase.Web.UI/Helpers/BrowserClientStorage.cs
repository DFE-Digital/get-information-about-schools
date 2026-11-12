using Edubase.Common;
using Edubase.Services.Core;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Edubase.Web.UI.Helpers
{
    public class BrowserClientStorage : IClientStorage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BrowserClientStorage(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string IPAddress => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        public string Get(string key)
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.Request?.Cookies?.Keys.Contains(key) == true ? context.Request.Cookies[key]?.Clean() : null;
        }

        public string Save(string key, string value)
        {
            var context = _httpContextAccessor.HttpContext;
            context?.Response?.Cookies.Append(key, value, new CookieOptions { SameSite = SameSiteMode.Strict });
            return value;
        }
    }
}
