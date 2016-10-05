using System.Web;
using System.Web.Routing;

namespace Edubase.Web.UI.Utils
{
    public class RequestContextWrapper : IRequestContext
    {
        public RequestContext GetContext()
        {
            return HttpContext.Current.Request.RequestContext;
        }
    }
}