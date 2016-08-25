using System.Web.Routing;

namespace Web.UI.Utils
{
    public interface IRequestContext
    {
        RequestContext GetContext();
    }
}