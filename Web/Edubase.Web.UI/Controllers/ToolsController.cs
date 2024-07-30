using Edubase.Services.Domain;
using Microsoft.Ajax.Utilities;
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
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly ICachedLookupService _lookup;
        private readonly IClientStorage _clientStorage;
        private readonly ILocalAuthoritySetRepository _localAuthoritySetRepository;
        private readonly IEstablishmentDownloadService _establishmentDownloadService;

        public ToolsController(ISecurityService securityService, IEstablishmentReadService establishmentReadService,
            IEstablishmentWriteService establishmentWriteService, ICachedLookupService lookup,
            IClientStorage clientStorage, ILocalAuthoritySetRepository localAuthoritySetRepository,
            IEstablishmentDownloadService establishmentDownloadService)
        {
            _securityService = securityService;
            _establishmentReadService = establishmentReadService;
            _establishmentWriteService = establishmentWriteService;
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
                UserCanCreateAcademyTrustGroup =
                    createGroupPermission.GroupTypes.Any(x =>
                        x == GT.MultiacademyTrust || x == GT.SingleacademyTrust),
                UserCanCreateChildrensCentreGroup =
                    createGroupPermission.GroupTypes.Any(x =>
                        x == GT.ChildrensCentresCollaboration || x == GT.ChildrensCentresGroup),
                UserCanCreateFederationGroup = createGroupPermission.GroupTypes.Any(x => x == GT.Federation),
                UserCanCreateSchoolTrustGroup = createGroupPermission.GroupTypes.Any(x => x == GT.Trust),
                UserCanCreateSecureSingleAcademyTrustGroup =
                    createGroupPermission.GroupTypes.Any(x => x == GT.SecureSingleAcademyTrust),
                UserCanCreateAcademySponsor = createGroupPermission.GroupTypes.Any(x => x == GT.SchoolSponsor),
                UserCanCreateEstablishment = createEstablishmentPermission.CanCreate,
                UserCanManageAcademyOpenings = User.InRole(AuthorizedRoles.CanManageAcademyOpenings),
                UserCanManageSecureAcademy16To19Openings =
                    User.InRole(AuthorizedRoles.CanManageSecureAcademy16To19Openings),
                UserCanBulkCreateAcademies = User.InRole(AuthorizedRoles.CanBulkCreateAcademies),
                UserCanMergeOrAmalgamateEstablishments = User.InRole(AuthorizedRoles.CanMergeEstablishments),
                UserCanBulkUpdateGovernors = User.InRole(AuthorizedRoles.CanBulkUpdateGovernors),
                UserCanBulkUpdateEstablishments = User.InRole(AuthorizedRoles.CanBulkUpdateEstablishments),
                UserCanApprove = User.InRole(AuthorizedRoles.CanApprove),
                UserCanConvertAcademyTrusts = User.InRole(AuthorizedRoles.CanManageAcademyTrusts),
                UserCanViewIndependentSchoolsSignificantDates =
                    User.InRole(AuthorizedRoles.CanSearchIndependentSchools),
                UserCanBulkCreateFreeSchools = User.InRole(AuthorizedRoles.CanBulkCreateFreeSchools),
                UserCanBulkAssociateEstabs2Groups = User.InRole(AuthorizedRoles.CanBulkAssociateEstabs2Groups),
                UserCanDownloadMATClosureReport = User.InRole(AuthorizedRoles.CanManageAcademyTrusts),
                UserCanManageNotifications = User.InRole(AuthorizedRoles.IsAdmin),
                UserCanManageNews = User.InRole(AuthorizedRoles.IsAdmin),
                UserCanViewLogs = User.InRole(AuthorizedRoles.IsAdmin),
                UserCanResetCaches = User.InRole(AuthorizedRoles.IsAdmin),
            };

            return View(viewModel);
        }

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanManageAcademyTrusts)]
        [Route("DownloadClosedTrustsInformation", Name = "DownloadClosedTrustsInformation")]
        public ActionResult DownloadClosedTrustsInformation() => View();

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanBulkCreateAcademies)]
        public ActionResult BulkAcademies() => View();

        [HttpPost, MvcAuthorizeRoles(AuthorizedRoles.CanBulkCreateAcademies), ValidateAntiForgeryToken]
        public async Task<ActionResult> BulkAcademies(BulkAcademiesViewModel model, int? removeUrn, int? editUrn,
            string action)
        {
            var establishmentTypeFullList = (await _lookup.EstablishmentTypesGetAllAsync()).ToList();
            model.ItemTypes = establishmentTypeFullList.ToSelectList();
            EstablishmentModel est = null;

            SelectListItem[] filteredItems = null;

            // validation
            if (action == "search")
            {
                if (model.SearchUrn == null)
                {
                    if (ModelState.ContainsKey(nameof(model.SearchUrn)) &&
                        ModelState[nameof(model.SearchUrn)].Errors.Any())
                    {
                        // remove the existing error, as we'll add a fresh one
                        ModelState[nameof(model.SearchUrn)].Errors.Clear();
                    }

                    ModelState.AddModelError(nameof(model.SearchUrn), "Please enter a valid URN");
                }
                else if (model.ItemsToAdd?.Any(x => x.Urn == model.SearchUrn) == true)
                {
                    ModelState.AddModelError(nameof(model.SearchUrn), "URN is a duplicate");
                }
                else
                {
                    var estCall = await _establishmentReadService.GetAsync((int) model.SearchUrn, User);
                    est = estCall.GetResult();
                    if (est == null)
                    {
                        ModelState.AddModelError(nameof(model.SearchUrn), "Please enter a valid URN");
                    }
                    else
                    {
                        filteredItems = (await GetFilteredBulkAcademyTypes((int) est.Urn, establishmentTypeFullList))
                            .ToSelectList(est?.TypeId)?.ToArray();
                        if (filteredItems?.Length == 0)
                        {
                            ModelState.AddModelError(nameof(model.SearchUrn), "Please enter a valid URN");
                        }
                    }
                }
            }

            if (action == "add")
            {
                model.FilteredItemTypes =
                    (await GetFilteredBulkAcademyTypes(model.FoundItem.Urn ?? 0, establishmentTypeFullList))
                    .ToSelectList(model.FoundItem.EstablishmentTypeId);

                if (model.FoundItem.EstablishmentTypeId == null)
                {
                    ModelState.AddModelError(nameof(model.FilteredItemTypes), "Please select an establishment type");
                }
            }

            // cancel either an original addition or an edit
            if (action == "cancel")
            {
                model.SearchUrn = null;
                model.FoundItem = null;
                ModelState.Clear();
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ModelState.Clear();

            // remove
            if (removeUrn != null)
            {
                model.ItemsToAdd?.RemoveAll(x => x.Urn == removeUrn);
                return View(model);
            }

            // edit/find
            if (editUrn != null || model.SearchUrn != null)
            {
                if (editUrn != null)
                {
                    var itm = model.ItemsToAdd?.First(x => x.Urn == editUrn);
                    model.FilteredItemTypes =
                        (await GetFilteredBulkAcademyTypes((int) editUrn, establishmentTypeFullList)).ToSelectList(
                            itm?.EstablishmentTypeId);
                    model.FoundItem = itm;
                    ViewBag.ButtonText = "Update establishment";
                }
                else
                {
                    model.FilteredItemTypes = filteredItems;
                    model.FoundItem = new BulkAcademyViewModel()
                    {
                        Urn = est.Urn,
                        Name = est.Name,
                        EstablishmentTypeId = est.TypeId,
                        OpeningDate = DateTime.Now,
                        Address = StringUtil.ConcatNonEmpties(", ",
                            est.Address_Line1,
                            est.Address_Locality,
                            est.Address_Line3,
                            est.Address_CityOrTown,
                            est.Address_PostCode)
                    };
                }

                return View(model);
            }

            // save/review list
            if (model.FoundItem != null)
            {
                if (model.ItemsToAdd == null)
                {
                    model.ItemsToAdd = new List<BulkAcademyViewModel>();
                }

                if (model.ItemsToAdd != null)
                {
                    model.ItemsToAdd.RemoveAll(x => x.Urn == model.FoundItem.Urn);

                    model.ItemsToAdd.Add(new BulkAcademyViewModel
                    {
                        Urn = model.FoundItem.Urn,
                        Name = model.FoundItem.Name,
                        EstablishmentTypeId = model.FoundItem.EstablishmentTypeId,
                        OpeningDate = model.FoundItem.OpeningDate,
                        Address = model.FoundItem.Address
                    });
                }

                model.FoundItem = null;
                return View(model);
            }

            // submit
            if (action == "create" && model.ItemsToAdd?.Any() == true)
            {
                var processResponse = await ProcessBulkAcademies(model.ItemsToAdd);
                model.ProgressGuid = processResponse.Item1;
                model.ItemsToAdd = processResponse.Item2;
                model.IsComplete = true;
            }

            return View(model);
        }


        private async Task<Tuple<Guid, List<BulkAcademyViewModel>>> ProcessBulkAcademies(
            List<BulkAcademyViewModel> itemsToAdd)
        {
            var responseGuid = Guid.Empty;

            // send the update request
            var newAcademies = new List<NewAcademyRequest>();
            foreach (var bvm in itemsToAdd)
            {
                newAcademies.Add(new NewAcademyRequest
                {
                    OpeningDate = bvm.OpeningDate ?? DateTime.Now,
                    TypeId = bvm.EstablishmentTypeId ?? 0,
                    PredecessorEstablishmentUrn = bvm.Urn ?? 0
                });
            }

            var submitCall = await _establishmentWriteService.BulkCreateAcademies(newAcademies.ToArray(), User);

            if (!submitCall.HasErrors)
            {
                var submitResponse = submitCall.GetResponse();
                responseGuid = submitResponse.Id;
            }


            // poll the process api until all have been completed
            var loopCount = 0;
            while (itemsToAdd.Count(x => x.ReplacedUrn == null) > 0)
            {
                var call = await _establishmentWriteService.GetBulkCreateAcademiesProgress(responseGuid, User);
                var response = call.GetResponse();
                if (response.IsComplete)
                {
                    var result = response.Result;
                    if (result.Length > 0)
                    {
                        for (var i = 0; i < result.Length; i++)
                        {
                            itemsToAdd[i].ReplacedUrn = result[i].Urn;
                        }
                    }
                }

                if (loopCount > 0)
                {
                    // pause before trying the non-complete items again
                    await Task.Delay(1000);
                }

                loopCount += 1;
            }

            return new Tuple<Guid, List<BulkAcademyViewModel>>(responseGuid, itemsToAdd);
        }

        private async Task<List<EstablishmentLookupDto>> GetFilteredBulkAcademyTypes(int urn,
            List<EstablishmentLookupDto> fullList)
        {
            var filteredList = fullList.ToList();
            foreach (var est in fullList)
            {
                var call = await _establishmentWriteService.ValidateBulkCreateAcademies(
                    new NewAcademyRequest[]
                    {
                        new NewAcademyRequest()
                        {
                            TypeId = est.Id, OpeningDate = DateTime.Now, PredecessorEstablishmentUrn = urn
                        }
                    }, User);

                if (call.HasErrors || (call.Response.Length > 0 && call.Response[0].HasErrors))
                {
                    filteredList.RemoveAll(x => x.Id == est.Id);
                }
            }

            return filteredList;
        }


        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanMergeEstablishments)]
        public async Task<ActionResult> MergersTool()
        {
            var settings =
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

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

        [HttpPost, EdubaseAuthorize, Route("ApiSessionRecorder", Name = "PostApiSessionRecorder"),
         ValidateAntiForgeryToken]
        public ActionResult ApiSessionRecorderToggle()
        {
            ViewBag.LastSessionId = _clientStorage.Get("ApiSessionId");
            if (_clientStorage.Get("ApiSessionId") == null)
                ViewBag.SessionId = _clientStorage.Save("ApiSessionId", Guid.NewGuid().ToString("N"));
            else ViewBag.SessionId = _clientStorage.Save("ApiSessionId", null);
            return View("ApiSessionRecorder");
        }

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanSearchIndependentSchools),
         Route("~/independent-schools", Name = "IndSchSearch")]
        public async Task<ActionResult> IndependentSchoolsSearch(IndSchoolsSearchViewModel viewModel)
        {
            await PopulateLookupData(viewModel);

            if (!string.Equals(viewModel.ActionName, IndSchoolsSearchViewModel.ActionSearch))
            {
                ModelState.Clear();
            }

            if (viewModel.MinDate.ToDateTime().HasValue && viewModel.MaxDate.ToDateTime().HasValue &&
                viewModel.MinDate.ToDateTime() > viewModel.MaxDate.ToDateTime())
                ModelState.AddModelError("date-range", "Please use a valid date range");

            if (ModelState.IsValid)
            {
                switch (viewModel.ActionName)
                {
                    case IndSchoolsSearchViewModel.ActionSearch:
                        return View("IndependentSchoolsSearchResults", viewModel.SetResults(
                            new PaginatedResult<EstablishmentSearchResultModel>(viewModel.Skip, viewModel.Take,
                                await _establishmentReadService.SearchAsync(
                                    await CreateIndSchoolSearchPayload(viewModel),
                                    User))));
                    case IndSchoolsSearchViewModel.ActionSaveSet:
                        return Redirect(string.Concat(Url.RouteUrl("CreatePredefinedLASet"), "?",
                            QueryStringHelper.ToQueryString(
                                IndSchoolsSearchViewModel.BindAliasForSelectedLocalAuthorityIds,
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

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanSearchIndependentSchools),
         Route("~/independent-schools/results-js", Name = "IndSchSearchResultsPartial")]
        public async Task<ActionResult> IndependentSchoolsSearchResults(IndSchoolsSearchViewModel viewModel)
        {
            await PopulateLookupData(viewModel);
            viewModel.Results = new PaginatedResult<EstablishmentSearchResultModel>(viewModel.Skip, viewModel.Take,
                await _establishmentReadService.SearchAsync(await CreateIndSchoolSearchPayload(viewModel), User));
            HttpContext.Response.Headers.Add("x-count", viewModel.Results.Count.ToString());
            return PartialView("_IndSchSearchResults", viewModel);
        }

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanAccessTools),
         Route("~/independent-schools/download", Name = "IndSchSearchResultsRequestDownload")]
        public async Task<ActionResult> IndependentSchoolsSearchDownload(IndSchoolsSearchViewModel viewModel)
        {
            var id = await _establishmentDownloadService.SearchWithDownloadGenerationAsync(
                new EstablishmentSearchDownloadPayload
                {
                    SearchPayload = await CreateIndSchoolSearchPayload(viewModel),
                    FileFormat = eFileFormat.XLSX,
                    DataSet = eDataSet.IEBT
                }, User);

            return RedirectToRoute("IndSchSearchResultsDownload", new { id, viewModel.Mode });
        }

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanAccessTools),
         Route("~/independent-schools/download/{id}", Name = "IndSchSearchResultsDownload")]
        public async Task<ActionResult> IndependentSchoolsSearchDownload(Guid id, string mode)
        {
            ViewBag.Subtitle = mode == IndSchoolsSearchViewModel.SpecifierDateOfActionGeneral
                ? "Download results for next general action required"
                : "Download results for next welfare action required";
            var model = await _establishmentDownloadService.GetDownloadGenerationProgressAsync(id, User);

            if (model.HasErrored)
                throw new Exception($"Download generation failed; Underlying error: '{model.Error}'");

            if (!model.IsComplete)
                return View("IndSchoolSearchDownload/PreparingFilePleaseWait", model);

            return View("IndSchoolSearchDownload/ReadyToDownload", model);
        }

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanAccessTools),
         Route("~/independent-schools/downloadAjax/{id}", Name = "IndSchSearchResultsDownloadAjax")]
        public async Task<ActionResult> IndependentSchoolsSearchDownloadAjax(Guid id, string mode)
        {
            var model = await _establishmentDownloadService.GetDownloadGenerationProgressAsync(id, User);

            if (model.HasErrored)
            {
                return Json(JsonConvert.SerializeObject(new
                {
                    status = "error", redirect = string.Concat("/independent-schools/download/", id)
                }));
            }


            return Json(
                JsonConvert.SerializeObject(new
                {
                    status = model.IsComplete, redirect = string.Concat("/independent-schools/download/", id)
                }), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanAccessTools),
         Route("~/independent-schools/predefined-local-authority-sets", Name = "PredefinedLASets")]
        public async Task<ActionResult> PredefinedLASets(PredefinedLASetsViewModel viewModel)
        {
            viewModel.LocalAuthorities = await _lookup.LocalAuthorityGetAllAsync();
            var items = (await _localAuthoritySetRepository.GetAllAsync()).Items.OrderBy(x => x.Title);
            viewModel.Results = new PaginatedResult<LocalAuthoritySet>(viewModel.Skip, 100, items.Count(),
                items.Skip(viewModel.Skip).Take(100).ToList());
            return View(viewModel);
        }

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanAccessTools),
         Route("~/independent-schools/predefined-local-authority-sets/create", Name = "CreatePredefinedLASet")]
        public async Task<ActionResult> CreatePredefinedLASet(PredefinedLASetViewModel viewModel)
        {
            ModelState.Clear();
            viewModel.LocalAuthorities = (await _lookup.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));
            return View("CreateEditPredefinedLASet", viewModel);
        }

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanAccessTools),
         Route("~/independent-schools/predefined-local-authority-sets/edit/{id}", Name = "EditPredefinedLASet")]
        public async Task<ActionResult> EditPredefinedLASet(string id)
        {
            var entity = await _localAuthoritySetRepository.GetAsync(id);
            return View("CreateEditPredefinedLASet",
                new PredefinedLASetViewModel
                {
                    Id = entity.RowKey,
                    SelectedLocalAuthorityIds = entity.Ids.ToList(),
                    Title = entity.Title,
                    LocalAuthorities = (await _lookup.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name)
                        .Select(x => new LookupItemViewModel(x))
                });
        }

        [HttpPost, MvcAuthorizeRoles(AuthorizedRoles.CanAccessTools),
         Route("~/independent-schools/predefined-local-authority-sets/edit/{id}", Name = "EditPredefinedLASetPost"),
         Route("~/independent-schools/predefined-local-authority-sets/create", Name = "CreatePredefinedLASetPost"),
            ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEditPredefinedLASet(PredefinedLASetViewModel viewModel)
        {
            if (ModelState.ContainsKey(nameof(viewModel.SuppressWarning)))
                ModelState.Remove(nameof(viewModel.SuppressWarning));

            if (ModelState.IsValid)
            {
                var sets = await _localAuthoritySetRepository.GetAllAsync();
                var duplicate = sets.Items
                    .Where(x => x.Title == viewModel.Title && (viewModel.IsNewEntity || viewModel.Id != x.RowKey))
                    .Select(x => x.RowKey).FirstOrDefault();

                if (duplicate == null || viewModel.SuppressWarning)
                {
                    var entity = viewModel.IsNewEntity
                        ? new LocalAuthoritySet()
                        : await _localAuthoritySetRepository.GetAsync(viewModel.Id);
                    entity.Title = viewModel.Title;
                    entity.Ids = viewModel.SelectedLocalAuthorityIds.ToArray();
                    if (viewModel.IsNewEntity) await _localAuthoritySetRepository.CreateAsync(entity);
                    else await _localAuthoritySetRepository.UpdateAsync(entity);

                    if (duplicate != null) await _localAuthoritySetRepository.DeleteAsync(duplicate);

                    if (viewModel.Referrer == "results")
                    {
                        return Redirect(string.Concat(Url.RouteUrl("IndSchSearch"), "?a=search&Mode=", viewModel.Mode,
                            "&",
                            QueryStringHelper.ToQueryString(
                                IndSchoolsSearchViewModel.BindAliasForSelectedLocalAuthorityIds,
                                viewModel.SelectedLocalAuthorityIds.ToArray())));
                    }

                    return RedirectToRoute("PredefinedLASets");
                }
                else if (duplicate != null) viewModel.WarningNameClash = true;
            }

            viewModel.LocalAuthorities = (await _lookup.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new LookupItemViewModel(x));

            return View("CreateEditPredefinedLASet", viewModel);
        }

        [HttpPost, MvcAuthorizeRoles(AuthorizedRoles.CanAccessTools),
         Route("~/independent-schools/predefined-local-authority-sets/delete/{id}", Name = "DeletePredefinedLASet"), ValidateAntiForgeryToken]
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
            if (viewModel.SelectedLocalAuthoritySetId.Clean() != null)
                laIds = (await _localAuthoritySetRepository.GetAsync(viewModel.SelectedLocalAuthoritySetId.Clean()))
                    .Ids;
            else laIds = viewModel.SelectedLocalAuthorityIds.ToArray();

            // Default to large min/max range, so that only those record _with_ a date are shown.
            Func<DateTime?, DateTime?> getMaxDate = (DateTime? dt) =>
                dt.HasValue ? dt : new DateTime(DateTime.UtcNow.Year + 100, 1, 1);
            Func<DateTime?, DateTime?> getMinDate = (DateTime? dt) =>
                dt.HasValue ? dt : new DateTime(DateTime.UtcNow.Year - 100, 1, 1);

            return new EstablishmentSearchPayload
            {
                Filters = new EstablishmentSearchFilters
                {
                    NextActionRequiredByWELMin =
                        viewModel.IsWelfareMode ? getMinDate(viewModel.MinDate.ToDateTime()) : null,
                    NextActionRequiredByWELMax =
                        viewModel.IsWelfareMode ? getMaxDate(viewModel.MaxDate.ToDateTime()) : null,
                    NextGeneralActionRequiredMin =
                        viewModel.IsGeneralMode ? getMinDate(viewModel.MinDate.ToDateTime()) : null,
                    NextGeneralActionRequiredMax =
                        viewModel.IsGeneralMode ? getMaxDate(viewModel.MaxDate.ToDateTime()) : null,
                    LocalAuthorityIds = laIds,
                    StatusIds = new[]
                    {
                        (int) eLookupEstablishmentStatus.Open,
                        (int) eLookupEstablishmentStatus.OpenButProposedToClose
                    }
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
