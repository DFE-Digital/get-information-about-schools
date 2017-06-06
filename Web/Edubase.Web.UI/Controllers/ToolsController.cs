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
                UserCanCreateEstablishment = createEstablishmentPermission.CanCreate,
                UserCanManageAcademyOpenings = User.InRole(EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO, EdubaseRoles.AP_AOS)
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