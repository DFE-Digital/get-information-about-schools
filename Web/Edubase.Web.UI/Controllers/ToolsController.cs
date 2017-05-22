using Edubase.Services.Security;
using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    using Filters;
    using System.Threading.Tasks;
    using GT = Services.Enums.eLookupGroupType;

    [RoutePrefix("Tools"), Route("{action=index}")]
    public class ToolsController : Controller
    {
        private readonly ISecurityService _securityService;

        public ToolsController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        // GET: Tools
        public async Task<ActionResult> Index()
        {
            var createGroupPermission = await _securityService.GetCreateGroupPermissionAsync(User);
            var createEstablishmentPermission = await _securityService.GetCreateEstablishmentPermissionAsync(User);

            var viewModel = new ToolsViewModel
            {
                UserCanCreateAcademyTrustGroup = createGroupPermission.GroupTypes.Any(x => x == GT.MultiacademyTrust || x == GT.SingleacademyTrust),
                UserCanCreateChildrensCentreGroup = createGroupPermission.GroupTypes.Any(x => x == GT.ChildrensCentresCollaboration || x == GT.ChildrensCentresGroup),
                UserCanCreateFederationGroup = createGroupPermission.GroupTypes.Any(x => x == GT.Federation),
                UserCanCreateSchoolTrustGroup = createGroupPermission.GroupTypes.Any(x => x == GT.Trust),
                UserCanCreateAcademySponsor = createGroupPermission.GroupTypes.Any(x => x == GT.SchoolSponsor),
                UserCanCreateEstablishment = createEstablishmentPermission.CanCreate
            };

            return View(viewModel);
        }
        [HttpGet, EdubaseAuthorize]
        public ActionResult BulkAcademies()
        {
            return View();
        }
        [HttpGet, EdubaseAuthorize]
        public ActionResult MergersTool()
        {
            return View();
        }

        [HttpGet, EdubaseAuthorize]
        public ActionResult ChangeApprovals()
        {
            return View();
        }

        [HttpGet, EdubaseAuthorize]
        public ActionResult CreateEstablishment()
        {
            return View();
        }

        [HttpGet, EdubaseAuthorize]
        public ActionResult ManageAcademyOpenings()
        {
            return View();
        }

        [HttpGet, EdubaseAuthorize]
        public ActionResult SearchChangeHistory()
        {
            return View();
        }

        [HttpGet, EdubaseAuthorize]
        public ActionResult EstablishmentBulkUpdate()
        {
            return View();
        }

        [HttpPost, EdubaseAuthorize]
        public ActionResult EstablishmentBulkUpdate(
            HttpPostedFileBase bulkfile,
            string fileType,
            string effectiveddateDay,
            string effectiveddateMonth,
            string effectiveddateYear)
        {
            ViewBag.globalError = false;
            ViewBag.invalidFileError = false;
            ViewBag.fileTypeError = false;
            ViewBag.missingFileError = false;

            ViewBag.fileTypeUnselected = fileType == "";

            if (bulkfile != null && bulkfile.ContentLength > 0)
            {
                ViewBag.fileName = Path.GetFileName(bulkfile.FileName);
                ViewBag.fileExtension = Path.GetExtension(bulkfile.FileName);
                ViewBag.invalidFileError = ViewBag.fileName == "invalid.csv";
                ViewBag.fileTypeError = !(ViewBag.fileExtension == ".csv" || ViewBag.fileExtension == ".xlsx");
            }
            else
            {
                ViewBag.missingFileError = true;
            }

            ViewBag.globalError = ViewBag.missingFileError || ViewBag.invalidFileError || ViewBag.fileTypeError /*|| ViewBag.fileTypeUnselected*/;
            ViewBag.fileError = ViewBag.missingFileError || ViewBag.fileTypeError;

            ViewBag.success = !(ViewBag.globalError || ViewBag.fileTypeUnselected);
            return View();
        }
    }
}