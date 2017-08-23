using Edubase.Services.Security;
using Edubase.Web.UI.Models;
using System.Linq;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    using Edubase.Services;
    using Edubase.Services.Establishments;
    using Edubase.Services.Lookup;
    using Filters;
    using Helpers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Threading.Tasks;
    using GT = Services.Enums.eLookupGroupType;
    using R = EdubaseRoles;

    [RoutePrefix("Tools"), Route("{action=index}"), EdubaseAuthorize]
    public class ToolsController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly ICachedLookupService _lookup;

        public ToolsController(ISecurityService securityService, IEstablishmentReadService establishmentReadService, ICachedLookupService lookup)
        {
            _securityService = securityService;
            _establishmentReadService = establishmentReadService;
            _lookup = lookup;
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
                UserCanMergeOrAmalgamateEstablishments = User.InRole(R.EDUBASE, R.ROLE_BACKOFFICE, R.EDUBASE_CMT, R.AP_AOS, R.APT, R.EFADO, R.FST, R.IEBT, R.SOU),
                UserCanBulkUpdateGovernors = User.InRole(R.EDUBASE_GROUP_MAT, R.ESTABLISHMENT, R.EFADO, R.ROLE_BACKOFFICE),
                UserCanBulkUpdateEstablishments = User.InRole(R.EDUBASE_CMT, R.EDUBASE, R.AP_AOS, R.APT, R.EDUBASE_CHILDRENS_CENTRE_POLICY, R.EFADO, R.EFAHNS, R.FST, R.IEBT, R.SOU, R.EDUBASE_LACCDO, R.LADO, R.LSU, R.UKRLP),
                UserCanApprove = User.InRole(R.EDUBASE, R.EDUBASE_CMT, R.ROLE_BACKOFFICE, R.AP_AOS, R.APT, R.EDUBASE_CHILDRENS_CENTRE_POLICY, R.EFADO, R.EFAHNS, R.FST, R.IEBT, R.SOU, R.EDUBASE_LACCDO, R.LADO),
                UserCanSearchChangeHistory = User.InRole(R.EDUBASE, R.EDUBASE_CMT, R.ROLE_BACKOFFICE, R.AP_AOS, R.APT, R.EDUBASE_CHILDRENS_CENTRE_POLICY, R.EFADO, R.EFAHNS, R.FST, R.IEBT, R.SOU, R.EDUBASE_LACCDO, R.LADO, R.OFSTED),
                UserCanConvertAcademyTrusts = User.InRole(R.EDUBASE, R.EDUBASE_CMT, R.ROLE_BACKOFFICE)
            };

            return View(viewModel);
        }

        [HttpGet, MvcAuthorizeRoles(R.AP_AOS, R.ROLE_BACKOFFICE, R.EFADO)]
        public ActionResult BulkAcademies() => View();

        [HttpGet, MvcAuthorizeRoles(R.AP_AOS, R.ROLE_BACKOFFICE, R.EFADO, R.SOU, R.IEBT)]
        public async Task<ActionResult> MergersTool()
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            var type2PhaseMap = _establishmentReadService.GetEstabType2EducationPhaseMap().AsInts();
            var type2PhaseMapJson = JsonConvert.SerializeObject(type2PhaseMap, Formatting.None, settings);

            var las = (await _lookup.LocalAuthorityGetAllAsync()).Select(x => new { x.Id, x.Name });
            var lasJson = JsonConvert.SerializeObject(las, Formatting.None, settings);

            var phases = (await _lookup.EducationPhasesGetAllAsync()).Select(x => new { x.Id, x.Name });
            var phasesJson = JsonConvert.SerializeObject(phases, Formatting.None, settings);

            var types = (await _lookup.EstablishmentTypesGetAllAsync()).Select(x => new { x.Id, x.Name });
            var typesJson = JsonConvert.SerializeObject(types, Formatting.None, settings);

            ViewBag.Type2PhaseMapJson = type2PhaseMapJson;
            ViewBag.LocalAuthoritiesJson = lasJson;
            ViewBag.TypesJson = typesJson;
            ViewBag.PhasesJson = phasesJson;

            return View();
        }
    }
}