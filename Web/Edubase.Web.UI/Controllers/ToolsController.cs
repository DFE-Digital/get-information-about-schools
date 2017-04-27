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
        public ActionResult Index()
        {
            var viewModel = new ToolsViewModel();
            var permission = _securityService.GetCreateGroupPermission(User);

            viewModel.UserCanCreateAcademyTrustGroup = permission.CanCreate((int)GT.MultiacademyTrust, permission.LocalAuthorityIds.FirstOrDefault()) 
                || permission.CanCreate((int)GT.SingleacademyTrust, permission.LocalAuthorityIds.FirstOrDefault());

            viewModel.UserCanCreateChildrensCentreGroup = permission.CanCreate((int)GT.ChildrensCentresCollaboration, permission.LocalAuthorityIds.FirstOrDefault())
                || permission.CanCreate((int)GT.ChildrensCentresGroup, permission.LocalAuthorityIds.FirstOrDefault());

            viewModel.UserCanCreateFederationGroup = permission.CanCreate((int)GT.Federation, permission.LocalAuthorityIds.FirstOrDefault());

            viewModel.UserCanCreateSchoolTrustGroup = permission.CanCreate((int)GT.Trust, permission.LocalAuthorityIds.FirstOrDefault());

            viewModel.UserCanCreateAcademySponsor = permission.CanCreate((int)GT.SchoolSponsor, permission.LocalAuthorityIds.FirstOrDefault());

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
                ViewBag.fileTypeError = !(ViewBag.fileExtension == ".csv" || ViewBag.fileExtension == ".xlxs");                
            }
            else
            {
                ViewBag.missingFileError = true;
            }

            ViewBag.globalError = ViewBag.missingFileError || ViewBag.invalidFileError || ViewBag.fileTypeError /*|| ViewBag.fileTypeUnselected*/;
            ViewBag.fileError = ViewBag.missingFileError || ViewBag.fileTypeError;

            ViewBag.success = !ViewBag.globalError;
            return View();
        }
    }
}