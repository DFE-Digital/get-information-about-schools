using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Models;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("ChangeHistory"), Route("{action=index}")]
    public class ChangeHistoryController : Controller
    {
        [HttpGet, EdubaseAuthorize, Route(Name = "ChangeHistory")]
        public ActionResult Index()
        {
            var vm = new ChangeHistoryViewModel();
            
            return View();
        }
    }
}