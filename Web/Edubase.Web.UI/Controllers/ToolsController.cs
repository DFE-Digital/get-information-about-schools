using Edubase.Services.Security;
using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
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
    }
}