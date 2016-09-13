using System.Web.Routing;

namespace Edubase.Web.UI.Utils
{
    public interface IRequestContext
    {
        RequestContext GetContext();
    }
}