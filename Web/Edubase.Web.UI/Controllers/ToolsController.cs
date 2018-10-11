using EdubaseRoles = Edubase.Services.Security.EdubaseRoles;

namespace Edubase.Web.UI.Controllers
{
    using Common;
    using Data.Entity;
    using Data.Repositories;
    using Filters;
    using Helpers;
    using Models;
    using Models.Tools;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Services;
    using Services.Core;
    using Services.Enums;
    using Services.Establishments;
    using Services.Establishments.Downloads;
    using Services.Establishments.Models;
    using Services.Establishments.Search;
    using Services.Lookup;
    using Services.Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using GT = Services.Enums.eLookupGroupType;
    using R = EdubaseRoles;

    [RoutePrefix("Tools"), Route("{action=index}"), EdubaseAuthorize]
    public class ToolsController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly ICachedLookupService _lookup;
        private readonly IClientStorage _clientStorage;
        private readonly ILocalAuthoritySetRepository _localAuthoritySetRepository;
        private readonly IEstablishmentDownloadService _establishmentDownloadService;

        public ToolsController(ISecurityService securityService, IEstablishmentReadService establishmentReadService, ICachedLookupService lookup, IClientStorage clientStorage, ILocalAuthoritySetRepository localAuthoritySetRepository, IEstablishmentDownloadService establishmentDownloadService)
        {
            _securityService = securityService;
            _establishmentReadService = establishmentReadService;
            _lookup = lookup;
            _clientStorage = clientStorage;
            _localAuthoritySetRepository = localAuthoritySetRepository;
            _establishmentDownloadService = establishmentDownloadService;
        }
        
        [Route(Name = "Tools")]
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
                UserCanApprove = User.InRole(R.EDUBASE, R.EDUBASE_CMT, R.ROLE_BACKOFFICE, R.AP_AOS, R.APT, R.EDUBASE_CHILDRENS_CENTRE_POLICY, R.EFADO, R.EFAHNS, R.FST, R.IEBT, R.SOU, R.EDUBASE_LACCDO, R.LADO, R.LSU),
                UserCanSearchChangeHistory = User.InRole(R.EDUBASE, R.EDUBASE_CMT, R.ROLE_BACKOFFICE, R.AP_AOS, R.APT, R.EDUBASE_CHILDRENS_CENTRE_POLICY, R.EFADO, R.EFAHNS, R.FST, R.IEBT, R.SOU, R.EDUBASE_LACCDO, R.LADO, R.OFSTED, R.LSU),
                UserCanConvertAcademyTrusts = User.InRole(R.EDUBASE, R.EDUBASE_CMT, R.ROLE_BACKOFFICE, R.AP_AOS),
                UserCanViewIndependentSchoolsSignificantDates = User.InRole(R.EDUBASE, R.EDUBASE_CMT, R.ROLE_BACKOFFICE, R.IEBT),
                UserCanBulkCreateFreeSchools = User.InRole(R.EDUBASE, R.ROLE_BACKOFFICE, R.EDUBASE_CMT, R.FST),
                UserCanBulkAssociateEstabs2Groups = User.InRole(R.EDUBASE, R.ROLE_BACKOFFICE, R.EDUBASE_CMT, R.AP_AOS),
                UserCanDownloadMATClosureReport = User.InRole(R.EDUBASE, R.ROLE_BACKOFFICE, R.EDUBASE_CMT, R.AP_AOS)
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

        [HttpGet, Route("ApiSessionRecorder", Name = "ApiSessionRecorder"), EdubaseAuthorize]
        public ActionResult ApiSessionRecorder()
        {
            ViewBag.SessionId = _clientStorage.Get("ApiSessionId");
            return View();
        }

        [HttpPost, EdubaseAuthorize, Route("ApiSessionRecorder", Name = "PostApiSessionRecorder"), ValidateAntiForgeryToken]
        public ActionResult ApiSessionRecorderToggle()
        {
            ViewBag.LastSessionId = _clientStorage.Get("ApiSessionId");
            if (_clientStorage.Get("ApiSessionId") == null) ViewBag.SessionId = _clientStorage.Save("ApiSessionId", Guid.NewGuid().ToString("N"));
            else ViewBag.SessionId = _clientStorage.Save("ApiSessionId", null);
            return View("ApiSessionRecorder");
        }

        [HttpGet, MvcAuthorizeRoles(R.ROLE_BACKOFFICE, R.IEBT), Route("~/independent-schools", Name = "IndSchSearch")]
        public async Task<ActionResult> IndependentSchoolsSearch(IndSchoolsSearchViewModel viewModel)
        {
            await PopulateLookupData(viewModel);

            if (!string.Equals(viewModel.ActionName, IndSchoolsSearchViewModel.ActionSearch))
            {
                ModelState.Clear();
            }

            if(viewModel.MinDate.ToDateTime().HasValue && viewModel.MaxDate.ToDateTime().HasValue && viewModel.MinDate.ToDateTime() > viewModel.MaxDate.ToDateTime())
                ModelState.AddModelError("date-range", "Please use a valid date range");
            
            if (ModelState.IsValid)
            {
                switch (viewModel.ActionName)
                {
                    case IndSchoolsSearchViewModel.ActionSearch:
                        return View("IndependentSchoolsSearchResults", viewModel.SetResults(
                            new PaginatedResult<EstablishmentSearchResultModel>(viewModel.Skip, viewModel.Take,
                                await _establishmentReadService.SearchAsync(await CreateIndSchoolSearchPayload(viewModel),
                                    User))));
                    case IndSchoolsSearchViewModel.ActionSaveSet:
                        return Redirect(string.Concat(Url.RouteUrl("CreatePredefinedLASet"), "?",
                            QueryStringHelper.ToQueryString(IndSchoolsSearchViewModel.BindAliasForSelectedLocalAuthorityIds,
                                viewModel.SelectedLocalAuthorityIds.ToArray())));
                    default:
                        break;
                }
            }
            
            viewModel.LocalAuthoritySets = (await _localAuthoritySetRepository.GetAllAsync()).Items
                .OrderBy(x => x.Title)
                .Select(x => new IndSchoolsSearchViewModel.LASetViewModel(x));

            return View(viewModel);
        }
        
        [HttpGet, MvcAuthorizeRoles(R.ROLE_BACKOFFICE, R.IEBT), Route("~/independent-schools/results-js", Name = "IndSchSearchResultsPartial")]
        public async Task<ActionResult> IndependentSchoolsSearchResults(IndSchoolsSearchViewModel viewModel)
        {
            await PopulateLookupData(viewModel);
            viewModel.Results = new PaginatedResult<EstablishmentSearchResultModel>(viewModel.Skip, viewModel.Take, 
                await _establishmentReadService.SearchAsync(await CreateIndSchoolSearchPayload(viewModel), User));
            HttpContext.Response.Headers.Add("x-count", viewModel.Results.Count.ToString());
            return PartialView("_IndSchSearchResults", viewModel);
        }

        [HttpGet, MvcAuthorizeRoles(R.ROLE_BACKOFFICE, R.IEBT), Route("~/independent-schools/download", Name = "IndSchSearchResultsRequestDownload")]
        public async Task<ActionResult> IndependentSchoolsSearchDownload(IndSchoolsSearchViewModel viewModel)
        {
            var id = await _establishmentDownloadService.SearchWithDownloadGenerationAsync(new EstablishmentSearchDownloadPayload
            {
                 SearchPayload= await CreateIndSchoolSearchPayload(viewModel),
                 FileFormat = eFileFormat.XLSX,
                 DataSet = eDataSet.IEBT
            }, User);
            
            return RedirectToRoute("IndSchSearchResultsDownload", new { id, viewModel.Mode });
        }

        [HttpGet, MvcAuthorizeRoles(R.ROLE_BACKOFFICE, R.IEBT), Route("~/independent-schools/download/{id}", Name = "IndSchSearchResultsDownload")]
        public async Task<ActionResult> IndependentSchoolsSearchDownload(Guid id, string mode)
        {
            ViewBag.Subtitle = mode == IndSchoolsSearchViewModel.SpecifierDateOfActionGeneral ? "Download results for next general action required": "Download results for next welfare action required";
            var model = await _establishmentDownloadService.GetDownloadGenerationProgressAsync(id, User);

            if (model.HasErrored)
                throw new Exception($"Download generation failed; Underlying error: '{model.Error}'");

            if (!model.IsComplete)
                return View("IndSchoolSearchDownload/PreparingFilePleaseWait", model);

            return View("IndSchoolSearchDownload/ReadyToDownload", model);
        }

        [HttpGet, MvcAuthorizeRoles(R.ROLE_BACKOFFICE, R.IEBT), Route("~/independent-schools/predefined-local-authority-sets", Name = "PredefinedLASets")]
        public async Task<ActionResult> PredefinedLASets(PredefinedLASetsViewModel viewModel)
        {
            viewModel.LocalAuthorities = await _lookup.LocalAuthorityGetAllAsync();
            var items = (await _localAuthoritySetRepository.GetAllAsync()).Items.OrderBy(x => x.Title);
            viewModel.Results =  new PaginatedResult<LocalAuthoritySet>(viewModel.Skip, 100, items.Count(), items.Skip(viewModel.Skip).Take(100).ToList());
            return View(viewModel);
        }

        [HttpGet, MvcAuthorizeRoles(R.ROLE_BACKOFFICE, R.IEBT), Route("~/independent-schools/predefined-local-authority-sets/create", Name = "CreatePredefinedLASet")]
        public async Task<ActionResult> CreatePredefinedLASet(PredefinedLASetViewModel viewModel)
        {
            ModelState.Clear();
            viewModel.LocalAuthorities = (await _lookup.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
            return View("CreateEditPredefinedLASet", viewModel);
        }

        [HttpGet, MvcAuthorizeRoles(R.ROLE_BACKOFFICE, R.IEBT), Route("~/independent-schools/predefined-local-authority-sets/edit/{id}", Name = "EditPredefinedLASet")]
        public async Task<ActionResult> EditPredefinedLASet(string id)
        {
            var entity = await _localAuthoritySetRepository.GetAsync(id);
            return View("CreateEditPredefinedLASet", new PredefinedLASetViewModel
            {
                Id = entity.RowKey,
                SelectedLocalAuthorityIds = entity.Ids.ToList(),
                Title = entity.Title,
                LocalAuthorities = (await _lookup.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x))
            });
        }

        [HttpPost, MvcAuthorizeRoles(R.ROLE_BACKOFFICE, R.IEBT), 
            Route("~/independent-schools/predefined-local-authority-sets/edit/{id}", Name = "EditPredefinedLASetPost"),
            Route("~/independent-schools/predefined-local-authority-sets/create", Name = "CreatePredefinedLASetPost")]
        public async Task<ActionResult> CreateEditPredefinedLASet(PredefinedLASetViewModel viewModel)
        {
            if(ModelState.ContainsKey(nameof(viewModel.SuppressWarning)))
                ModelState.Remove(nameof(viewModel.SuppressWarning));
            
            if (ModelState.IsValid)
            {
                var sets = await _localAuthoritySetRepository.GetAllAsync();
                var duplicate = sets.Items.Where(x => x.Title == viewModel.Title && (viewModel.IsNewEntity || viewModel.Id != x.RowKey)).Select(x => x.RowKey).FirstOrDefault();

                if (duplicate == null || viewModel.SuppressWarning)
                {
                    var entity = viewModel.IsNewEntity ? new LocalAuthoritySet() : await _localAuthoritySetRepository.GetAsync(viewModel.Id);
                    entity.Title = viewModel.Title;
                    entity.Ids = viewModel.SelectedLocalAuthorityIds.ToArray();
                    if (viewModel.IsNewEntity) await _localAuthoritySetRepository.CreateAsync(entity);
                    else await _localAuthoritySetRepository.UpdateAsync(entity);

                    if (duplicate != null) await _localAuthoritySetRepository.DeleteAsync(duplicate);

                    if (viewModel.Referrer == "results")
                    {
                        return Redirect(string.Concat(Url.RouteUrl("IndSchSearch"), "?a=search&Mode=", viewModel.Mode, "&",
                            QueryStringHelper.ToQueryString(IndSchoolsSearchViewModel.BindAliasForSelectedLocalAuthorityIds,
                                viewModel.SelectedLocalAuthorityIds.ToArray())));
                    }
                    return RedirectToRoute("PredefinedLASets");
                }
                else if (duplicate != null) viewModel.WarningNameClash = true;
            }

            viewModel.LocalAuthorities = (await _lookup.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));

            return View("CreateEditPredefinedLASet", viewModel);
        }

        [HttpPost, MvcAuthorizeRoles(R.ROLE_BACKOFFICE, R.IEBT), Route("~/independent-schools/predefined-local-authority-sets/delete/{id}", Name = "DeletePredefinedLASet")]
        public async Task<ActionResult> DeletePredefinedLASet(string id)
        {
            await _localAuthoritySetRepository.DeleteAsync(id);
            return RedirectToRoute("PredefinedLASets");
        }

        private async Task PopulateLookupData(IndSchoolsSearchViewModel viewModel)
        {
            viewModel.LocalAuthorities = await _lookup.LocalAuthorityGetAllAsync();
            viewModel.EstablishmentTypes = await _lookup.EstablishmentTypesGetAllAsync();
            viewModel.EstablishmentStatuses = await _lookup.EstablishmentStatusesGetAllAsync();
        }

        private async Task<EstablishmentSearchPayload> CreateIndSchoolSearchPayload(IndSchoolsSearchViewModel viewModel)
        {
            int[] laIds;
            if (viewModel.SelectedLocalAuthoritySetId.Clean() != null) laIds = (await _localAuthoritySetRepository.GetAsync(viewModel.SelectedLocalAuthoritySetId.Clean())).Ids;
            else laIds = viewModel.SelectedLocalAuthorityIds.ToArray();

            // Default to large min/max range, so that only those record _with_ a date are shown.
            Func<DateTime?, DateTime?> getMaxDate = (DateTime? dt) => dt.HasValue ? dt : new DateTime(DateTime.UtcNow.Year + 100, 1, 1);
            Func<DateTime?, DateTime?> getMinDate = (DateTime? dt) => dt.HasValue ? dt : new DateTime(DateTime.UtcNow.Year - 100, 1, 1);

            return new EstablishmentSearchPayload
            {
                Filters = new EstablishmentSearchFilters
                {
                    NextActionRequiredByWELMin = viewModel.IsWelfareMode ? getMinDate(viewModel.MinDate.ToDateTime()) : null,
                    NextActionRequiredByWELMax = viewModel.IsWelfareMode ? getMaxDate(viewModel.MaxDate.ToDateTime()) : null,
                    NextGeneralActionRequiredMin = viewModel.IsGeneralMode ? getMinDate(viewModel.MinDate.ToDateTime()) : null,
                    NextGeneralActionRequiredMax = viewModel.IsGeneralMode ? getMaxDate(viewModel.MaxDate.ToDateTime()) : null,
                    LocalAuthorityIds = laIds,
                    StatusIds = new[] { (int) eLookupEstablishmentStatus.Open, (int) eLookupEstablishmentStatus.OpenButProposedToClose }
                },
                Skip = viewModel.Skip,
                Take = 50,
                Select = new List<string>
                    {
                        nameof(EstablishmentSearchResultModel.Name),
                        nameof(EstablishmentSearchResultModel.LocalAuthorityId),
                        nameof(EstablishmentSearchResultModel.Address_CityOrTown),
                        nameof(EstablishmentSearchResultModel.StatusId),
                        nameof(EstablishmentSearchResultModel.TypeId),
                        nameof(EstablishmentSearchResultModel.NextGeneralActionRequired),
                        nameof(EstablishmentSearchResultModel.NextActionRequiredByWEL)
                    }
            };
        }
    }
}
