using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class PrototypeController : Controller
    {
        public ActionResult Index(string viewName) => View(viewName);
    }
}