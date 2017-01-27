using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Prototype"), Route("{action=index}")]
    public class PrototypeController : Controller
    {
        public ActionResult Index(string viewName) => View(viewName);
    }
}