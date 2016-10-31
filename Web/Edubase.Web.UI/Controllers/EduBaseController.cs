using Edubase.Data.Entity;
using Edubase.Web.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class EduBaseController : Controller
    {
        private Lazy<ApplicationDbContext> _dc = new Lazy<ApplicationDbContext>(ApplicationDbContext.Create);

        protected ApplicationDbContext DataContext => _dc.Value;

        protected new JsonCamelCaseResult Json(object data) => new JsonCamelCaseResult(data, JsonRequestBehavior.AllowGet);


        protected override void Dispose(bool disposing)
        {
            if ((_dc?.IsValueCreated).GetValueOrDefault())
            {
                _dc.Value.Dispose();
                _dc = null;
            }
            base.Dispose(disposing);
        }
    }
}