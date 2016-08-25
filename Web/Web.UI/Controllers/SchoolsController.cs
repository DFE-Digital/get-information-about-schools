using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Services.Schools;
using Web.UI.Models;

namespace Web.UI.Controllers
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

            var model = accessibleSchoolIds.Select(id => _schoolService.GetSchoolDetails(id));

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _schoolService.GetSchoolDetails(id);

            return View(model);
        }
    }
}