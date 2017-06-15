using Edubase.Services.Security;
using Edubase.Web.UI.Models;
using System.Linq;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    using Filters;
    using Helpers;
    using System.Threading.Tasks;
    using GT = Services.Enums.eLookupGroupType;
    using R = EdubaseRoles;

    [RoutePrefix("Tools"), Route("{action=index}"), EdubaseAuthorize]
    public class ToolsController : Controller
    {
        private readonly ISecurityService _securityService;

        public ToolsController(ISecurityService securityService)
        {
            _securityService = securityService;
        }
        
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
                UserCanManageAcademyOpenings = User.InRole(R.ROLE_BACKOFFICE, R.EFADO, R.AP_AOS),
                UserCanBulkCreateAcademies = User.InRole(R.ROLE_BACKOFFICE, R.EFADO, R.AP_AOS),
                UserCanMergeOrAmalgamateEstablishments = User.InRole(R.AP_AOS, R.ROLE_BACKOFFICE, R.EFADO, R.SOU, R.IEBT),
                UserCanBulkUpdateGovernors = User.InRole(R.EDUBASE_GROUP_MAT, R.ESTABLISHMENT, R.EFADO, R.ROLE_BACKOFFICE)
            };

            return View(viewModel);
        }
        [HttpGet, MvcAuthorizeRoles(R.AP_AOS, R.ROLE_BACKOFFICE, R.EFADO)]
        public ActionResult BulkAcademies() => View();

        [HttpGet, MvcAuthorizeRoles(R.AP_AOS, R.ROLE_BACKOFFICE, R.EFADO, R.SOU, R.IEBT)]
        public ActionResult MergersTool() => View();
    }
}