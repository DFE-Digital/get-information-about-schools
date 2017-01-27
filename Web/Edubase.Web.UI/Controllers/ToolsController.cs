using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Tools"), Route("{action=index}")]
    public class ToolsController : Controller
    {
        // GET: Tools
        public ActionResult Index()
        {
            return View();
        }
    }
}