using System.Linq;
using System.Web;
using Edubase.Services.Texuna.Glimpse;
using Glimpse.AspNet.Extensibility;
using Glimpse.Core.Extensibility;
using Microsoft.AspNet.Identity;

namespace Edubase.Web.UI.Glimpse
{
    public class CurrentUserApiTraceTab : AspNetTab
    {
        public override object GetData(ITabContext context)
        {
            var httpContext = HttpContext.Current;
            var userIp = httpContext?.Request?.UserHostAddress;
            var userId = httpContext?.User?.Identity?.GetUserId();
            var userName = httpContext?.User?.Identity?.GetUserName();

            return new
            {
                CurrentIp = userIp,
                CurrentUserId = userId,
                CurrentUserName = userName,
                ApiData = ApiTrace.Data.Where(d => string.Equals(d.ClientIpAddress, userIp) && string.Equals(d.UserId, userId) && string.Equals(d.UserName, userName)).Select(d => new { d.StartTime, d.Method, d.Url, d.ResponseCode, d.Request, d.Response, d.DurationMillis})
            };
        }

        public override string Name => "Current User Api Trace";
    }
}
