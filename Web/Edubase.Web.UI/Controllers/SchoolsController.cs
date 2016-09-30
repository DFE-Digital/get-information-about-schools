using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Services.Schools;
using Edubase.Web.UI.Models;
using Edubase.Data.Entity;
using System.Dynamic;

namespace Edubase.Web.UI.Controllers
{
    [Authorize]
    public class SchoolsController : Controller
    {
        private readonly ISchoolPermissions _schoolPermissions;
        private readonly ISchoolService _schoolService;

        public SchoolsController(ISchoolPermissions schoolPermissions, ISchoolService schoolService)
        {
            _schoolPermissions = schoolPermissions;
            _schoolService = schoolService;
        }

        // GET: School
        public ActionResult Index()
        {
            var accessibleSchoolIds = _schoolPermissions.GetAccessibleSchoolIds().ToArray();

            if (accessibleSchoolIds.Length == 1)
            {
                return RedirectToAction("Details", new {id = accessibleSchoolIds.Single()});
            }

            using (var dc = new ApplicationDbContext())
            {
                var model = accessibleSchoolIds.Select(id =>
                {
                    dynamic o = new ExpandoObject();
                    o.SCHNAME = dc.Establishments.FirstOrDefault(x => x.Urn == id)?.Name;
                    o.id = id;
                    return o;
                });
                return View(model);
            }
        }

        public ActionResult Details(int id)
        {
            using (var dc = new ApplicationDbContext())
            {
                var model = dc.Establishments.FirstOrDefault(x => x.Urn == id);
                return View(model);
            }
        }
    }
}