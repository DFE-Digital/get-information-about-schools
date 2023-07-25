using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Services;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.EditPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
using Edubase.Services.ExternalLookup;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Services.Texuna.Lookup;
using Edubase.Web.Resources;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Areas.Establishments.Models.Validators;
using Edubase.Web.UI.Areas.Governors.Controllers;
using Edubase.Web.UI.Controllers;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Validation;
using FluentValidation.Mvc;
using MoreLinq;
using ET = Edubase.Services.Enums.eLookupEstablishmentType;
using CreateSteps = Edubase.Web.UI.Areas.Establishments.Models.CreateEstablishmentViewModel.eEstabCreateSteps;
using ViewModel = Edubase.Web.UI.Models.EditEstablishmentModel;

namespace Edubase.Web.UI.Areas.Establishments.Controllers
{
    [RouteArea("Establishments"), RoutePrefix("Establishment")]
    public class EstablishmentController : EduBaseController
    {
        private readonly ICachedLookupService _cachedLookupService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly IGroupReadService _groupReadService;
        private readonly IMapper _mapper;
        private readonly IResourcesHelper _resourcesHelper;
        private readonly IExternalLookupService _externalLookupService;
        private readonly IUserDependentLookupService _lookupService;

        private readonly ISecurityService _securityService;
        private readonly Lazy<string[]> _formKeys;
        private readonly Dictionary<string, string> validationFieldMapping = new Dictionary<string, string>
        {
            {"address_Line1", "Address.Line1"},
            {"address_CityOrTown", "Address.CityOrTown" },
            {"address_CountyId", "Address.County" },
            {"address_PostCode", "Address.PostCode" },
            {"headFirstName", "ManagerFirstName" },
            {"headLastName", "ManagerLastName" },
            {"contact_EmailAddress", "CentreEmail" },
            {"contact_TelephoneNumber", "Telephone" },
            {"ccOperationalHoursId", "OperationalHoursId" },
            {"ccUnder5YearsOfAgeCount", "NumberOfUnderFives" },
            {"ccGovernanceId", "GovernanceId" },
            {"ccGovernanceDetail", "GovernanceDetail" },
            {"ccDisadvantagedAreaId", "DisadvantagedAreaId" },
            {"ccDirectProvisionOfEarlyYearsId", "DirectProvisionOfEarlyYears" },
            {"statusId", "EstablishmentStatusId" }
        };

        public EstablishmentController(IEstablishmentReadService establishmentReadService,
            IGroupReadService groupReadService,
            IMapper mapper,
            IEstablishmentWriteService establishmentWriteService,
            ICachedLookupService cachedLookupService,
            IResourcesHelper resourcesHelper,
            ISecurityService securityService,
            IExternalLookupService externalLookupService,
            IUserDependentLookupService lookupService)
        {
            _cachedLookupService = cachedLookupService;
            _establishmentReadService = establishmentReadService;
            _establishmentWriteService = establishmentWriteService;
            _groupReadService = groupReadService;
            _mapper = mapper;
            _resourcesHelper = resourcesHelper;
            _securityService = securityService;
            _externalLookupService = externalLookupService;
            _lookupService = lookupService;

            _formKeys = new Lazy<string[]>(
                () => Request?.Form?.AllKeys.Select(x => x.GetPart(".")).Distinct().ToArray(),
                LazyThreadSafetyMode.PublicationOnly);
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{urn:int}/Link/{linkid?}", Name = "EditEstabLink"),
            Route("Edit/{urn:int}/Link/Create/{urnToLink:int}", Name = "CreateEstabLink")]
        public async Task<ActionResult> AddEditLinkAsync(int urn, int? linkId, int? urnToLink)
        {
            var viewModel = new EditEstablishmentLinksViewModel();
            await PopulateEstablishmentPageViewModel(viewModel, urn, "links");

            if (linkId.HasValue)
            {
                viewModel.Links = (await _establishmentReadService.GetLinkedEstablishmentsAsync(urn, User)).Select(x => new LinkedEstabViewModel(x)).ToList();
                viewModel.ActiveRecord = viewModel.Links.First(x => x.Id == linkId);
                viewModel.ActiveRecord.Address = await (await _establishmentReadService.GetAsync(viewModel.ActiveRecord.Urn.Value, User)).GetResult().GetAddressAsync(_cachedLookupService);
            }
            else
            {
                var domainModel = (await _establishmentReadService.GetAsync(urnToLink.Value, User)).GetResult();
                viewModel.ActiveRecord = new LinkedEstabViewModel
                {
                    Address = await domainModel.GetAddressAsync(_cachedLookupService),
                    EstablishmentName = domainModel.Name,
                    Urn = domainModel.Urn
                };
            }

            viewModel.LinkTypeList = await _cachedLookupService.EstablishmentLinkTypesGetAllAsync();
            viewModel.HydrateStateToken();

            return View("AddEditLink", viewModel);
        }

        [HttpPost, EdubaseAuthorize, Route("Edit/{urn:int}/Link/{linkid?}", Name = "SaveEstabLink"),
            Route("Edit/{urn:int}/Link/Create/{urnToLink:int}")]
        public async Task<ActionResult> AddEditLinkAsync(EditEstablishmentLinksViewModel deltaViewModel)
        {
            if (deltaViewModel.Act == "delete")
            {
                return await DeleteLinkAsync(deltaViewModel);
            }
            else
            {
                var viewModel = UriHelper.DeserializeUrlToken<EditEstablishmentLinksViewModel>(deltaViewModel.StateToken); // avoids more API calls, which are very slow.
                viewModel.ActiveRecord.LinkDateEditable = deltaViewModel.ActiveRecord.LinkDateEditable;
                viewModel.ActiveRecord.LinkTypeId = deltaViewModel.ActiveRecord.LinkTypeId;
                viewModel.ActiveRecord.CreateReverseLink = deltaViewModel.ActiveRecord.CreateReverseLink;
                viewModel.ActiveRecord.ReverseLinkDateEditable = deltaViewModel.ActiveRecord.ReverseLinkDateEditable;
                viewModel.ActiveRecord.ReverseLinkTypeId = deltaViewModel.ActiveRecord.ReverseLinkTypeId;
                viewModel.ActiveRecord.ReverseLinkSameDate = deltaViewModel.ActiveRecord.ReverseLinkSameDate;
                viewModel.HydrateStateToken();

                if (ModelState.IsValid)
                {
                    var link = new LinkedEstablishmentModel();
                    var set = (await _establishmentReadService.GetLinkedEstablishmentsAsync(deltaViewModel.Urn.Value, User)).ToList();

                    if (deltaViewModel.ActiveRecord.Id.HasValue)
                    {
                        link = set.FirstOrDefault(x => x.Id == deltaViewModel.ActiveRecord.Id);
                        Guard.IsNotNull(link, () => new Exception($"Link with id {deltaViewModel.ActiveRecord.Id} was not found in the set"));
                    }

                    link.LinkTypeId = viewModel.ActiveRecord.LinkTypeId;
                    link.LinkDate = viewModel.ActiveRecord.LinkDateEditable.ToDateTime();
                    link.Urn = viewModel.ActiveRecord.Urn;

                    if (!link.Id.HasValue)
                    {
                        set.Add(link);
                    }

                    var apiResponse = await _establishmentWriteService.SaveLinkedEstablishmentsAsync(deltaViewModel.Urn.Value, set.ToArray(), User);

                    if (apiResponse.HasErrors)
                    {
                        apiResponse.Errors.ForEach(x => ModelState.AddModelError(x.Fields ?? string.Empty, x.GetMessage()));
                    }
                    else
                    {
                        if (viewModel.ActiveRecord.CreateReverseLink)
                        {
                            await _establishmentWriteService.AddLinkedEstablishmentAsync(link.Urn.Value, viewModel.Urn.Value, viewModel.ActiveRecord.ReverseLinkTypeId.Value,
                                (viewModel.ActiveRecord.ReverseLinkDateEditable.ToDateTime() ?? viewModel.ActiveRecord.LinkDateEditable.ToDateTime()).Value, User);
                        }
                        return RedirectToRoute("EditEstabLinks", new { id = deltaViewModel.Urn, saved = true });
                    }
                }
                return View("AddEditLink", viewModel);
            }
        }

        [HttpGet, EdubaseAuthorize, Route("Create", Name = "CreateEstablishment")]
        public async Task<ActionResult> Create()
        {
            var permission = await _securityService.GetCreateEstablishmentPermissionAsync(User);
            if (!permission.CanCreate)
            {
                throw new PermissionDeniedException("Current principal does not have the required permissions.");
            }

            var viewModel = new CreateChildrensCentreViewModel
            {
                CreateEstablishmentPermission = await _securityService.GetCreateEstablishmentPermissionAsync(User),
                Type2PhaseMap = _establishmentReadService.GetEstabType2EducationPhaseMap().AsInts(),
                Address = new AddressViewModel(),
            };
            await PopulateCCSelectLists(viewModel);
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Create")]
        public async Task<ActionResult> Create(CreateChildrensCentreViewModel viewModel, bool JsDisabled = false)
        {
            viewModel.CreateEstablishmentPermission = await _securityService.GetCreateEstablishmentPermissionAsync(User);
            viewModel.Type2PhaseMap = _establishmentReadService.GetEstabType2EducationPhaseMap().AsInts();
            var routeComplete = viewModel.ActionStep == CreateSteps.Completed;

            await PopulateCCSelectLists(viewModel);
            if (viewModel.EstablishmentTypeId != null)
            {
                //Bugfix - ensures repopulation of available phases on step 2
                var phaseMap = _establishmentReadService.GetEstabType2EducationPhaseMap().AsInts()[viewModel.EstablishmentTypeId.Value];
                viewModel.EducationPhases = (await _cachedLookupService.EducationPhasesGetAllAsync()).Where(x => phaseMap.Contains(x.Id)).ToSelectList(viewModel.EducationPhaseId);
            }

            ModelState.Remove(nameof(viewModel.PreviousStep));
            ModelState.Remove(nameof(viewModel.CurrentStep));
            ModelState.Remove(nameof(viewModel.ActionStep));

            if (viewModel.ActionStep < viewModel.CurrentStep)
            {
                viewModel.ActionStep = viewModel.CurrentStep == CreateSteps.CreateEntry ? CreateSteps.PhaseOfEducation : viewModel.CurrentStep;
                viewModel.CurrentStep = viewModel.PreviousStep;
                viewModel.PreviousStep = viewModel.PreviousStep - 1;
                return View(viewModel);
            }

            var isNameEntryOk = viewModel.LocalAuthorityId != null && viewModel.Name != null && viewModel.EstablishmentTypeId != null;
            var isPhaseOfEducationOk = viewModel.EducationPhaseId != null && viewModel.GenerateEstabNumber != null;

            if (viewModel.EstablishmentTypeId == 41 && routeComplete && isNameEntryOk)
            {
                return ModelState.IsValid ? await CreateChildrensCentre(viewModel) : View(viewModel);
            }

            if (ModelState.IsValid && !routeComplete)
            {
                viewModel.PreviousStep = viewModel.CurrentStep;
                viewModel.CurrentStep = viewModel.ActionStep;
            }

            if (ModelState.IsValid && viewModel.EstablishmentTypeId == 41 && viewModel.ActionStep == CreateSteps.PhaseOfEducation && !routeComplete && isNameEntryOk)
            {
                viewModel.CurrentStep = CreateSteps.CreateEntry;
                viewModel.ActionStep = CreateSteps.Completed;
                //need to escape here to redraw the screen and collect additional data
                return View(viewModel);
            }

            if (ModelState.IsValid && viewModel.ActionStep == CreateSteps.EstabNumber && isPhaseOfEducationOk)
            {
                viewModel.CurrentStep = CreateSteps.EstabNumber;
                viewModel.ActionStep = CreateSteps.Completed;
                return View(viewModel);
            }

            if (ModelState.IsValid && viewModel.ActionStep != CreateSteps.EstabNumber && !routeComplete)
            {
                if (viewModel.ActionStep == CreateSteps.PhaseOfEducation && isNameEntryOk)
                {
                    viewModel.ActionStep = viewModel.EstablishmentTypeId != 41
                        ? CreateSteps.EstabNumber
                        : CreateSteps.Completed;
                }
            }

            if (ModelState.IsValid && routeComplete)  //attempt to prevent end of route processing until final control is posted
            {
                var apiModel = new EstablishmentModel
                {
                    Name = viewModel.Name,
                    EstablishmentNumber = viewModel.EstablishmentNumber.ToInteger(),
                    EducationPhaseId = viewModel.EducationPhaseId,
                    TypeId = viewModel.EstablishmentTypeId,
                    LocalAuthorityId = viewModel.LocalAuthorityId,
                    CCLAContactDetail = new ChildrensCentreLocalAuthorityDto(),
                    IEBTModel = new IEBTModel(),
                    StatusId = (int) eLookupEstablishmentStatus.ProposedToOpen
                };

                if (viewModel.EstablishmentTypeId == (int) ET.SixthFormCentres) // story: 25821
                {
                    apiModel.StatutoryLowAge = 0;
                    apiModel.StatutoryHighAge = 0;
                }

                var validation = await _establishmentWriteService.ValidateCreateAsync(apiModel, true, User);
                ApplyCreateEstabValidationErrors(validation);

                if (ModelState.IsValid)
                {
                    viewModel.SetWarnings(validation);
                    ModelState.Remove(nameof(viewModel.ProcessedWarnings));
                }
                else
                {
                    // go back to estab number
                    viewModel.ActionStep = CreateSteps.Completed;
                    viewModel.CurrentStep = CreateSteps.EstabNumber;
                    viewModel.PreviousStep = CreateSteps.PhaseOfEducation;
                }

                if (ModelState.IsValid && !viewModel.WarningsToProcess.Any())
                {
                    var response = await _establishmentWriteService.CreateNewAsync(apiModel, viewModel.GenerateEstabNumber.GetValueOrDefault(), User);
                    if (response.Success)
                    {
                        return RedirectToAction(nameof(Details), new { id = response.Response });
                    }
                    else
                    {
                        foreach (var error in response.Errors)
                        {
                            ModelState.AddModelError(error.Fields, error.GetMessage());
                        }
                    }
                }
            }
            return View(viewModel);
        }

        [HttpGet, Route("Details/{id:int}", Name = "EstabDetails")]
        public async Task<ActionResult> Details(int id, string searchQueryString = "", eLookupSearchSource searchSource = eLookupSearchSource.Establishments,
            int approved = 0, int pending = 0, int skip = 0, string sortBy = null, bool saved = false, string confirmed = null)
        {
            ViewBag.ApprovedCount = approved;
            ViewBag.PendingApprovalCount = pending;
            ViewBag.ShowSaved = saved;
            ViewBag.Confirmed = confirmed;

            var viewModel = new EstablishmentDetailViewModel(_externalLookupService)
            {
                IsUserLoggedOn = User.Identity.IsAuthenticated,
                SearchQueryString = searchQueryString,
                SearchSource = searchSource
            };

            var result = await _establishmentReadService.GetAsync(id, User);
            if (result.ReturnValue == null)
            {
                return HttpNotFound();
            }

            viewModel.Establishment = result.ReturnValue;
            viewModel.TabWarnings = new TabWarningsModel(viewModel.Establishment.TypeId);

            await Task.WhenAll(
                PopulateLinkedEstablishments(id, viewModel),
                PopulateChangeHistory(id, viewModel, skip, 100, sortBy),
                PopulateGroups(id, viewModel),
                PopulateDisplayPolicies(viewModel),
                PopulateEditPermissions(viewModel),
                PopulateLookupNames(viewModel),
                PopulateGovernors(viewModel));

          viewModel.AgeRangeToolTip = viewModel.Establishment.TypeId.OneOfThese(ET.OnlineProvider)
                ? _resourcesHelper.GetResourceStringForEstablishment("AgeRangeOnlineProvider", (eLookupEstablishmentTypeGroup?) viewModel.Establishment.EstablishmentTypeGroupId, User)
                : _resourcesHelper.GetResourceStringForEstablishment("AgeRange", (eLookupEstablishmentTypeGroup?) viewModel.Establishment.EstablishmentTypeGroupId, User);

            viewModel.AgeRangeToolTipLink = viewModel.Establishment.TypeId.Equals((int)ET.AcademySecure16to19)
                ? string.Empty
                : _resourcesHelper.GetResourceStringForEstablishment("AgeRangeLink", (eLookupEstablishmentTypeGroup?) viewModel.Establishment.EstablishmentTypeGroupId, User);

            viewModel.SchoolCapacityToolTip = _resourcesHelper.GetResourceStringForEstablishment("SchoolCapacity", (eLookupEstablishmentTypeGroup?) viewModel.Establishment.EstablishmentTypeGroupId, User);

            viewModel.SchoolCapacityToolTipLink = viewModel.Establishment.TypeId.Equals((int)ET.AcademySecure16to19)
                ? string.Empty
                : _resourcesHelper.GetResourceStringForEstablishment("SchoolCapacityLink", (eLookupEstablishmentTypeGroup?) viewModel.Establishment.EstablishmentTypeGroupId, User);
               
            return View(viewModel);
        }

        [HttpGet, Route("Details/{id:int}/Governance/Changes", Name = "EstabDetailGovChangeHistory"), EdubaseAuthorize]
        public async Task<ActionResult> GovernanceChangeHistoryAsync(int id, int skip = 0, string sortBy = null)
        {
            var result = await _establishmentReadService.GetAsync(id, User);

            if (result.ReturnValue == null)
            {
                return HttpNotFound();
            }

            var changes = await _establishmentReadService.GetGovernanceChangeHistoryAsync(id, skip, 100, sortBy, User);

            var viewModel = new EstablishmentDetailViewModel(_externalLookupService)
            {
                Establishment = result.ReturnValue,
                ChangeHistory = changes
            };

            await Task.WhenAll(
                PopulateDisplayPolicies(viewModel)
            );


            viewModel.LegalParentGroup = GetLegalParent(viewModel.Establishment.Urn.Value,
                await _groupReadService.GetAllByEstablishmentUrnAsync(viewModel.Establishment.Urn.Value, User), User);

            return View("GovernanceChangeHistory", viewModel);
        }

        [HttpHead, Route("Details/{urn:int}", Name = "EstabDetailsHead")]
        public async Task<ActionResult> DetailsHead(int urn)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            var result = await _establishmentReadService.GetAsync(urn, User);
            return result.ReturnValue == null ? HttpNotFound() : new HttpStatusCodeResult(200);
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}", Name = "EditEstablishmentDetail")]
        public async Task<ActionResult> EditDetails(int? id, string addrtok)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var viewModel = await CreateEditViewModel(id, vm =>
            {
                if (addrtok.Clean() != null)
                {
                    AddOrReplaceAddressFromUrlToken(addrtok, vm);
                }
            });
            viewModel.SelectedTab = "details";
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}")]
        public async Task<ActionResult> EditDetails(ViewModel model) => await SaveEstablishment(model);

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/Helpdesk")]
        public async Task<ActionResult> EditHelpdesk(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var viewModel = await CreateEditViewModel(id);
            if (!viewModel.TabDisplayPolicy.Helpdesk)
            {
                throw new PermissionDeniedException();
            }

            viewModel.SelectedTab = "helpdesk";
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}/Helpdesk")]
        public async Task<ActionResult> EditHelpdesk(ViewModel model) => await SaveEstablishment(model);

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/IEBT")]
        public async Task<ActionResult> EditIEBT(int? id)
        {
            if (id.HasValue)
            {
                var viewModel = await CreateEditViewModel(id);
                if (!viewModel.TabDisplayPolicy.IEBT)
                {
                    throw new PermissionDeniedException();
                }

                viewModel.SelectedTab = "iebt";
                return View("EditIEBT", viewModel);
            }

            return HttpNotFound();
        }

        [HttpGet, EdubaseAuthorize, Route("Proprietor/Add/{urn:int}/{counter:int}")]
        public async Task<PartialViewResult> ProprietorAdd(int urn, int? counter)
        {
            var emptyProprietor = new ProprietorViewModel
            {
                Counties = (await _cachedLookupService.CountiesGetAllAsync()).ToSelectList(),
            };
            ViewBag.ShowCounter = counter;

            var editPolicy = (await _establishmentReadService.GetEditPolicyByUrnAsync(urn, User)).EditPolicy;
            ViewData.Add("displayPolicy", editPolicy.IEBTDetail.Proprietors);

            return PartialView("Partials/_EditProprietors", emptyProprietor);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}/IEBT")]
        public async Task<ActionResult> EditIEBT(ViewModel model) => await SaveEstablishment(model);

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/Links", Name = "EditEstabLinks")]
        public async Task<ActionResult> EditLinks(int? id, bool saved = false)
        {
            ViewBag.ShowSaved = saved;

            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var viewModel = new EditEstablishmentLinksViewModel();
            await PopulateEstablishmentPageViewModel(viewModel, id.Value, "links");
            viewModel.Links = (await _establishmentReadService.GetLinkedEstablishmentsAsync(id.Value, User)).Select(x => new LinkedEstabViewModel(x)).ToList();
            await Task.WhenAll(viewModel.Links.Select(async x => x.LinkTypeName = await _cachedLookupService.GetNameAsync(() => x.LinkTypeId)));

            return View("EditLinks", viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/Location/{locationField?}")]
        public async Task<ActionResult> EditLocation(int? id, string locationField = null)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var viewModel = await CreateEditViewModel(id);
            if (!viewModel.TabDisplayPolicy.Location)
            {
                throw new PermissionDeniedException();
            }

            viewModel.SelectedTab = "location";
            viewModel.LocationEditField = locationField ?? string.Empty;
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}/Location/")]
        public async Task<ActionResult> EditLocation(ViewModel model)
        {
            var targetViewModel = await CreateEditViewModel(model.Urn);

            SetProperty(targetViewModel, model, m => m.RSCRegionId);
            SetProperty(targetViewModel, model, m => m.GovernmentOfficeRegionId);
            SetProperty(targetViewModel, model, m => m.AdministrativeDistrictId);
            SetProperty(targetViewModel, model, m => m.AdministrativeDistrictName);
            SetProperty(targetViewModel, model, m => m.AdministrativeWardId);
            SetProperty(targetViewModel, model, m => m.AdministrativeWardName);
            SetProperty(targetViewModel, model, m => m.ParliamentaryConstituencyId);
            SetProperty(targetViewModel, model, m => m.ParliamentaryConstituencyName);
            SetProperty(targetViewModel, model, m => m.UrbanRuralId);
            SetProperty(targetViewModel, model, m => m.GSSLAId);
            SetProperty(targetViewModel, model, m => m.GSSLAName);
            SetProperty(targetViewModel, model, m => m.Easting);
            SetProperty(targetViewModel, model, m => m.Northing);
            SetProperty(targetViewModel, model, m => m.MSOAName);
            SetProperty(targetViewModel, model, m => m.MSOAId);
            SetProperty(targetViewModel, model, m => m.LSOAName);
            SetProperty(targetViewModel, model, m => m.LSOAId);

            targetViewModel.ActionSpecifier = model.ActionSpecifier;
            targetViewModel.SelectedTab = model.SelectedTab;
            targetViewModel.OverrideCRProcess = model.OverrideCRProcess;
            targetViewModel.ChangeEffectiveDate = model.ChangeEffectiveDate;

            return await SaveEstablishment(targetViewModel);
        }

        [HttpPost, EdubaseAuthorize, Route("Confirm/{urn:int}", Name = "EstablishmentConfirmUpToDate")]
        public async Task<ActionResult> EstablishmentConfirmUpToDateAsync(int urn, bool showBanner = false)
        {
            await _establishmentWriteService.ConfirmAsync(urn, User);
            return RedirectToRoute("EstabDetails", new { id = urn, saved = showBanner, confirmed = "Details" });
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{urn:int}/Address/{target}", Name = "AddOrReplaceEstablishmentAddress")]
        public async Task<ActionResult> AddOrReplaceEstablishmentAddressAsync(int urn, string target)
        {
            var viewModel = new AddOrReplaceAddressViewModel((await _cachedLookupService.NationalitiesGetAllAsync()).ToSelectList(Constants.COUNTRY_ID_UK),
                (await _cachedLookupService.CountiesGetAllAsync()).ToSelectList(), target);
            await PopulateEstablishmentPageViewModel(viewModel, urn, "details");
            return View("AddOrReplaceAddress", viewModel);
        }

        [HttpPost, EdubaseAuthorize, Route("Edit/{urn:int}/Address/{target}", Name = "AddOrReplaceEstablishmentAddressPost")]
        public async Task<ActionResult> AddOrReplaceEstablishmentAddressPostAsync(int urn, string target, AddOrReplaceAddressViewModel viewModel)
        {
            ModelState.Clear();

            if (viewModel.ActionName == null)
            {
                viewModel.Step = viewModel.CountryId == Constants.COUNTRY_ID_UK ? "enterpostcode" : "editaddress";
            }
            else if (viewModel.ActionName == "find-address")
            {
                if (viewModel.CountryId == Constants.COUNTRY_ID_UK)
                {
                    viewModel.LookupAddresses = await _establishmentReadService.GetAddressesByPostCodeAsync(viewModel.PostCode, User);
                    if (viewModel.LookupAddresses.Any())
                    {
                        viewModel.Step = "selectaddress";
                    }
                    else
                    {
                        ModelState.AddModelError("PostCode", "We could not find any addresses matching that postcode");
                    }
                }
                else
                {
                    viewModel.Step = "editaddress";
                }
            }
            else if (viewModel.ActionName == "address-selected" && viewModel.SelectedUPRN != null)
            {
                var address = viewModel.LookupAddresses.FirstOrDefault(x => x.UPRN == viewModel.SelectedUPRN);
                viewModel.Street = address.Street;
                viewModel.Town = address.Town;
                viewModel.PostCode = address.PostCode;
                viewModel.Easting = address.Easting;
                viewModel.Northing = address.Northing;
                viewModel.Step = "editaddress";
            }
            else if (viewModel.ActionName == "replace-address")
            {
                var payload = UriHelper.SerializeToUrlToken(viewModel);
                return RedirectToRoute("EditEstablishmentDetail", new { id = urn, addrtok = payload });
            }

            await PopulateEstablishmentPageViewModel(viewModel, urn, "details");
            return View("AddOrReplaceAddress", viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/Links/Search", Name = "EditEstabLinks_SearchForEstablishment")]
        public async Task<ActionResult> SearchForEstablishment(int? id, SearchForEstablishmentViewModel viewModel)
        {
            viewModel = viewModel ?? new SearchForEstablishmentViewModel();
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            await PopulateEstablishmentPageViewModel(viewModel, id.Value, "links");

            if (viewModel.DoSearch)
            {
                var results = await new SearchForEstablishmentViewModelValidator(_establishmentReadService, User).ValidateAsync(viewModel);
                results.AddToModelState(ModelState, string.Empty);

                if (ModelState.IsValid)
                {
                    return RedirectToRoute("CreateEstabLink", new { urn = id.Value, urnToLink = viewModel.SearchUrn.ToInteger().Value });
                }
            }

            return View("AddEditLink_FindEstablishment", viewModel);
        }

        /// <summary>
        /// This all needs to be refactored, but not before a major release ;-)
        /// </summary>
        /// <param name="establishmentUrn"></param>
        /// <param name="parentGroups"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        internal static GroupModel GetLegalParent(int establishmentUrn, IEnumerable<GroupModel> parentGroups, IPrincipal principal)
        {
            var parentGroup = parentGroups.FirstOrDefault(g => g.GroupTypeId == (int) eLookupGroupType.SingleacademyTrust);
            if (parentGroup != null)
            {
                return parentGroup;
            }

            parentGroup = parentGroups.FirstOrDefault(g => g.GroupTypeId == (int) eLookupGroupType.MultiacademyTrust);
            if (parentGroup != null)
            {
                return parentGroup;
            }

            parentGroup = parentGroups.FirstOrDefault(g => g.GroupTypeId == (int) eLookupGroupType.Trust);
            return parentGroup ?? parentGroups.FirstOrDefault();
        }

        private void ApplyCreateEstabValidationErrors(ValidationEnvelopeDto validation)
        {
            foreach (var error in validation.Errors)
            {
                if (validationFieldMapping.ContainsKey(error.Fields))
                {
                    error.Fields = validationFieldMapping[error.Fields];
                }
            }

            validation.ApplyToModelState(ControllerContext, true);
        }

        private async Task<ActionResult> CreateChildrensCentre(CreateChildrensCentreViewModel model)
        {
            model.EducationPhaseId = 1;

            var newEstablishment = new EstablishmentModel
            {
                TypeId = 41,
                LocalAuthorityId = model.LocalAuthorityId,
                Name = model.Name,
                OpenDate = model.OpenDate.ToDateTime(),
                Address_Line1 = model.Address.Line1,
                Address_Line2 = model.Address.Line2,
                Address_Line3 = model.Address.Line3,
                Address_CityOrTown = model.Address.CityOrTown,
                Address_CountyId = model.Address.County,
                Address_PostCode = model.Address.PostCode,
                HeadFirstName = model.ManagerFirstName,
                HeadLastName = model.ManagerLastName,
                Contact_EmailAddress = model.CentreEmail,
                Contact_TelephoneNumber = model.Telephone,
                CCOperationalHoursId = model.OperationalHoursId,
                CCUnder5YearsOfAgeCount = model.NumberOfUnderFives,
                CCGovernanceId = model.GovernanceId,
                CCGovernanceDetail = model.GovernanceDetail,
                CCDisadvantagedAreaId = model.DisadvantagedAreaId,
                CCDirectProvisionOfEarlyYearsId = model.DirectProvisionOfEarlyYears,
                CCPhaseTypeId = 3,
                StatusId = model.EstablishmentStatusId,
                IEBTModel = new IEBTModel()
            };

            var validation = await _establishmentWriteService.ValidateCreateAsync(newEstablishment, true, User);
            ApplyCreateEstabValidationErrors(validation);

            if (ModelState.IsValid)
            {
                var response = await _establishmentWriteService.CreateNewAsync(newEstablishment, true, User);

                if (response.Success)
                {
                    return RedirectToAction(nameof(Details), new { id = response.Response });
                }
                else if (response.Errors.Any(x => x.Code == "establishment.with.same.name.la.postcode.found"))
                {
                    ModelState.AddModelError(nameof(model.Name), "Please enter a different establishment name");
                    ModelState.AddModelError(nameof(model.LocalAuthorityId), "Please enter a different local authority");
                    ModelState.AddModelError("Address.PostCode", "Please enter a different postcode");
                }
                else
                {
                    response.ApplyToModelState(ControllerContext);
                }
            }

            await PopulateCCSelectLists(model);
            return View(model);
        }

        private async Task<ViewModel> CreateEditViewModel(int? id, Action<ViewModel> preprocessViewModel = null)
        {
            var domainModel = (await _establishmentReadService.GetAsync(id.Value, User)).GetResult();
            var viewModel = _mapper.Map<ViewModel>(domainModel);
            _mapper.Map(domainModel.IEBTModel, viewModel);

            viewModel.EditPolicy = (await _establishmentReadService.GetEditPolicyAsync(domainModel, User)).EditPolicy;
            viewModel.DisplayPolicy = await _establishmentReadService.GetDisplayPolicyAsync(domainModel, User);
            viewModel.TabDisplayPolicy = new TabDisplayPolicy(domainModel, viewModel.EditPolicy, User);
            viewModel.CanOverrideCRProcess = User.IsInRole(AuthorizedRoles.IsAdmin);
            viewModel.SENIds = viewModel.SENIds ?? new int[0];
            viewModel.LocationEditField = string.Empty;
            viewModel.EmptyEmailFields = new List<string>();

            preprocessViewModel?.Invoke(viewModel);

            viewModel.EditPolicy.AdditionalAddresses = CanUserDefineAdditionalAddresses(domainModel.TypeId.GetValueOrDefault());

            await PopulateSelectLists(viewModel);

            if (domainModel.AdministrativeDistrictId.HasValue)
            {
                var District = (await _cachedLookupService.AdministrativeDistrictsGetAllAsync()).FirstOrDefault(x => x.Id == domainModel.AdministrativeDistrictId.Value);
                viewModel.AdministrativeDistrictName = District?.Name;
                viewModel.AdministrativeDistrictId = domainModel.AdministrativeDistrictId;
            }

            if (domainModel.AdministrativeWardId.HasValue)
            {
                var lookup = (await _cachedLookupService.AdministrativeWardsGetAllAsync()).FirstOrDefault(x => x.Id == domainModel.AdministrativeWardId.Value);
                viewModel.AdministrativeWardName = $"{lookup?.Name} [{lookup?.Code}]";
                viewModel.AdministrativeWardId = domainModel.AdministrativeWardId;
            }

            if (domainModel.ParliamentaryConstituencyId.HasValue)
            {
                var lookup = (await _cachedLookupService.ParliamentaryConstituenciesGetAllAsync()).FirstOrDefault(x => x.Id == domainModel.ParliamentaryConstituencyId.Value);
                viewModel.ParliamentaryConstituencyName = lookup?.Name;
                viewModel.ParliamentaryConstituencyId = domainModel.ParliamentaryConstituencyId;
            }

            if (domainModel.GSSLAId.HasValue)
            {
                var lookup = (await _cachedLookupService.GSSLAGetAllAsync()).FirstOrDefault(x => x.Id == domainModel.GSSLAId.Value);
                viewModel.GSSLAName = lookup?.Name;
                viewModel.GSSLAId = domainModel.GSSLAId;
            }

            if (domainModel.MSOAId.HasValue)
            {
                var MSOA = (await _cachedLookupService.MSOAsGetAllAsync()).FirstOrDefault(x => x.Id == domainModel.MSOAId.Value);
                viewModel.MSOAName = $"{MSOA?.Name} [{MSOA?.Code}]";
                viewModel.MSOAId = domainModel.MSOAId;
            }

            if (domainModel.LSOAId.HasValue)
            {
                var LSOA = (await _cachedLookupService.LSOAsGetAllAsync()).FirstOrDefault(x => x.Id == domainModel.LSOAId.Value);
                viewModel.LSOAName = $"{LSOA?.Name} [{LSOA?.Code}]";
                viewModel.LSOAId = domainModel.LSOAId;
            }
            return viewModel;
        }

        public bool CanUserDefineAdditionalAddresses(int typeId)
            => typeId.OneOfThese(ET.NonmaintainedSpecialSchool,
                ET.OtherIndependentSchool,
                ET.OtherIndependentSpecialSchool) && User.InRole(AuthorizedRoles.CanDefineAdditionalAddresses);

        private async Task<ActionResult> DeleteLinkAsync(EditEstablishmentLinksViewModel deltaViewModel)
        {
            await _establishmentWriteService.DeleteLinkedEstablishmentAsync(deltaViewModel.Urn.Value, deltaViewModel.ActiveRecord.Id.Value, User);
            var reverseLinks = await _establishmentReadService.GetLinkedEstablishmentsAsync(deltaViewModel.ActiveRecord.Urn.Value, User);

            var reverseLink = reverseLinks.FirstOrDefault(x => x.Urn == deltaViewModel.Urn);
            if (reverseLink != null)
            {
                await _establishmentWriteService.DeleteLinkedEstablishmentAsync(deltaViewModel.ActiveRecord.Urn.Value, reverseLink.Id.Value, User);
            }

            return RedirectToRoute("EditEstabLinks", new { id = deltaViewModel.Urn });
        }

        /// <summary>
        /// Transfers property values from the view model to the domain model
        /// where the value is passed into the form; i.e., the key is present.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="domainModel"></param>
        /// <param name="form"></param>
        private void MapToDomainModel(ViewModel viewModel, EstablishmentModel domainModel)
        {
            var mappedModel = _mapper.Map<EstablishmentModel>(viewModel);
            // we would normally rely on automapper here, but because we're not passing the full model back (only the tab data)
            // it clears the model if we just use automapper.

            // to correct that, we loop through each of the fields returned, and pluck those from the mappedModel into the more populated, historic, domainModel

            var keys = _formKeys.Value;
            var properties = ReflectionHelper.GetProperties(domainModel, writeableOnly: true);
            properties = properties.Where(x => keys.Contains(x)).ToList();

            var viewModelProperties = ReflectionHelper.GetProperties(mappedModel);

            foreach (var item in properties.Intersect(viewModelProperties))
            {
                var value = ReflectionHelper.GetPropertyValue(mappedModel, item);
                ReflectionHelper.SetProperty(domainModel, item, value);
            }

            if (keys.Contains("SENList"))
            {
                domainModel.SENIds = viewModel.SENIds ?? new int[0];
            }

            domainModel.AdditionalAddresses = viewModel.AdditionalAddresses.ToArray();
        }

        private void MapToDomainModelIEBT(ViewModel viewModel, EstablishmentModel domainModel)
        {
            var mappedModel = _mapper.Map<IEBTModel>(viewModel);
            // we would normally rely on automapper here, but because we're not passing the full model back (only the tab data)
            // it clears the model if we just use automapper.

            // to correct that, we loop through each of the fields returned, and pluck those from the mappedModel into the more populated, historic, domainModel

            var keys = _formKeys.Value;
            var properties = ReflectionHelper.GetProperties(domainModel.IEBTModel, writeableOnly: true);
            properties = properties.Where(x => keys.Contains(x)).ToList();

            var viewModelProperties = ReflectionHelper.GetProperties(mappedModel);

            foreach (var item in properties.Intersect(viewModelProperties))
            {
                var value = ReflectionHelper.GetPropertyValue(mappedModel, item);
                ReflectionHelper.SetProperty(domainModel.IEBTModel, item, value);
            }
        }

        private async Task PopulateCCSelectLists(CreateChildrensCentreViewModel viewModel)
        {
            viewModel.PhaseId = 3;
            viewModel.OperationalHoursOptions = (await _cachedLookupService.CCOperationalHoursGetAllAsync()).ToSelectList();
            viewModel.GovernanceOptions = (await _cachedLookupService.CCGovernanceGetAllAsync()).ToSelectList();
            viewModel.DisadvantagedAreaOptions = (await _cachedLookupService.CCDisadvantagedAreasGetAllAsync()).ToSelectList();
            viewModel.DirectProvisionOfEarlyYearsOptions = (await _cachedLookupService.DirectProvisionOfEarlyYearsGetAllAsync()).ToSelectList();
            viewModel.EstablishmentStatusOptions = (await _lookupService.EstablishmentStatusesGetAllAsync(User)).ToSelectList();
            viewModel.Address.Counties = (await _cachedLookupService.CountiesGetAllAsync()).ToSelectList();
            viewModel.Phases = (await _cachedLookupService.CCPhaseTypesGetAllAsync()).ToSelectList();
            await PopulateSelectLists(viewModel);
        }

        private async Task PopulateChangeHistory(int id, EstablishmentDetailViewModel viewModel, int skip, int take, string sortBy)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    viewModel.ChangeHistory = await _establishmentReadService.GetChangeHistoryAsync(id, skip, take, sortBy, User);
                }
                catch
                {
                    // KHD: Texuna will sometimes return an API error.  I have been asked to ignore it.
                }
            }
        }

        private async Task PopulateDisplayPolicies(EstablishmentDetailViewModel viewModel)
        {
            viewModel.DisplayPolicy = await _establishmentReadService.GetDisplayPolicyAsync(viewModel.Establishment, User);
            viewModel.TabDisplayPolicy = new TabDisplayPolicy(viewModel.Establishment, viewModel.DisplayPolicy, User);
        }

        private async Task PopulateEditPermissions(EstablishmentDetailViewModel viewModel)
        {
            viewModel.UserCanEdit = await _establishmentReadService.CanEditAsync(viewModel.Establishment.Urn.Value, User);
            var editPolicyEnvelope = await _establishmentReadService.GetEditPolicyAsync(viewModel.Establishment, User);
            viewModel.TabEditPolicy = new TabEditPolicy(viewModel.Establishment, editPolicyEnvelope.EditPolicy, User);
        }

        private async Task PopulateEstablishmentPageViewModel(IEstablishmentPageViewModel viewModel, int urn, string selectedTabName)
        {
            viewModel.SelectedTab = selectedTabName;
            viewModel.Urn = urn;
            var domainModel = (await _establishmentReadService.GetAsync(urn, User)).GetResult();
            var editPolicy = (await _establishmentReadService.GetEditPolicyAsync(domainModel, User)).EditPolicy;
            viewModel.TabDisplayPolicy = new TabDisplayPolicy(domainModel, editPolicy, User);
            viewModel.Name = domainModel.Name;

            if (domainModel.TypeId.HasValue)
            {
                viewModel.TypeName = await _cachedLookupService.GetNameAsync(() => domainModel.TypeId);
            }

            viewModel.LegalParentGroup = GetLegalParent(viewModel.Urn.Value, await _groupReadService.GetAllByEstablishmentUrnAsync(viewModel.Urn.Value, User), User);
        }

        private async Task PopulateGovernors(EstablishmentDetailViewModel viewModel)
        {
            var governorsController = DependencyResolver.Current.GetService<GovernorController>();
            viewModel.GovernorsGridViewModel = await governorsController.CreateGovernorsViewModel(establishmentModel: viewModel.Establishment, user: User);
        }

        private async Task PopulateGroups(int id, EstablishmentDetailViewModel viewModel)
        {
            viewModel.Groups = await _groupReadService.GetAllByEstablishmentUrnAsync(id, User);
            viewModel.LegalParentGroup = GetLegalParent(id, viewModel.Groups, User);
        }

        private async Task PopulateLinkedEstablishments(int id, EstablishmentDetailViewModel viewModel)
        {
            viewModel.LinkedEstablishments = (await _establishmentReadService.GetLinkedEstablishmentsAsync(id, User)).Select(x => new LinkedEstabViewModel(x)).ToList();
            await Task.WhenAll(viewModel.LinkedEstablishments.Select(async x => x.LinkTypeName = await _cachedLookupService.GetNameAsync(() => x.LinkTypeId)));
        }

        private async Task PopulateLookupNames(EstablishmentDetailViewModel vm)
        {
            var c = _cachedLookupService;
            vm.ReligiousCharacterName = await c.GetNameAsync(() => vm.Establishment.ReligiousCharacterId);
            vm.DioceseName = await c.GetNameAsync(() => vm.Establishment.DioceseId);
            vm.ReligiousEthosName = await c.GetNameAsync(() => vm.Establishment.ReligiousEthosId);
            vm.ProvisionBoardingName = await c.GetNameAsync(() => vm.Establishment.ProvisionBoardingId);
            vm.BoardingEstabName = await c.GetNameAsync(() => vm.Establishment.IEBTModel.BoardingEstablishmentId, nameof(IEBTModel));
            vm.AccommodationChangedName = await c.GetNameAsync(() => vm.Establishment.IEBTModel.AccommodationChangedId, nameof(IEBTModel));
            vm.QualityAssuranceBodyName = await c.GetNameAsync(() => vm.Establishment.QualityAssuranceBodyNameId);
            vm.EstablishmentAccreditedName = await c.GetNameAsync(() => vm.Establishment.EstablishmentAccreditedId);
            vm.ProvisionNurseryName = await c.GetNameAsync(() => vm.Establishment.ProvisionNurseryId);
            vm.ProvisionOfficialSixthFormName = await c.GetNameAsync(() => vm.Establishment.ProvisionOfficialSixthFormId);
            vm.Section41ApprovedName = await c.GetNameAsync(() => vm.Establishment.Section41ApprovedId);
            vm.ReasonEstablishmentOpenedName = await c.GetNameAsync(() => vm.Establishment.ReasonEstablishmentOpenedId);
            vm.ReasonEstablishmentClosedName = await c.GetNameAsync(() => vm.Establishment.ReasonEstablishmentClosedId);
            vm.CCOperationalHoursName = await c.GetNameAsync(() => vm.Establishment.CCOperationalHoursId);
            vm.CCGovernanceName = await c.GetNameAsync(() => vm.Establishment.CCGovernanceId);
            vm.CCDeliveryModelName = await c.GetNameAsync(() => vm.Establishment.CCDeliveryModelId);
            vm.CCGroupLeadName = await c.GetNameAsync(() => vm.Establishment.CCGroupLeadId);
            vm.CCPhaseTypeName = await c.GetNameAsync(() => vm.Establishment.CCPhaseTypeId);
            vm.CCDisadvantagedAreaName = await c.GetNameAsync(() => vm.Establishment.CCDisadvantagedAreaId);
            vm.CCDirectProvisionOfEarlyYearsName = await c.GetNameAsync(() => vm.Establishment.CCDirectProvisionOfEarlyYearsId);
            vm.ProvisionSpecialClassesName = await c.GetNameAsync(() => vm.Establishment.ProvisionSpecialClassesId);
            vm.TeenageMothersProvisionName = await c.GetNameAsync(() => vm.Establishment.TeenageMothersProvisionId);
            vm.ChildcareFacilitiesName = await c.GetNameAsync(() => vm.Establishment.ChildcareFacilitiesId);
            vm.PRUSENName = await c.GetNameAsync(() => vm.Establishment.PRUSENId);
            vm.PRUEBDName = await c.GetNameAsync(() => vm.Establishment.PRUEBDId);
            vm.PruFulltimeProvisionName = await c.GetNameAsync(() => vm.Establishment.PruFulltimeProvisionId);
            vm.PruEducatedByOthersName = await c.GetNameAsync(() => vm.Establishment.PruEducatedByOthersId);
            vm.TypeOfResourcedProvisionName = await c.GetNameAsync(() => vm.Establishment.TypeOfResourcedProvisionId);
            vm.BSOInspectorateName = await c.GetNameAsync(() => vm.Establishment.BSOInspectorateId);
            vm.InspectorateName = await c.GetNameAsync(() => vm.Establishment.InspectorateId);
            vm.IndependentSchoolTypeName = await c.GetNameAsync(() => vm.Establishment.IEBTModel.IndependentSchoolTypeId);
            vm.RSCRegionName = await c.GetNameAsync(() => vm.Establishment.RSCRegionId);
            vm.GovernmentOfficeRegionName = await c.GetNameAsync(() => vm.Establishment.GovernmentOfficeRegionId);
            vm.AdministrativeDistrictName = await c.GetNameAsync(() => vm.Establishment.AdministrativeDistrictId);
            vm.AdministrativeWardName = await c.GetNameAsync(() => vm.Establishment.AdministrativeWardId);
            vm.ParliamentaryConstituencyName = await c.GetNameAsync(() => vm.Establishment.ParliamentaryConstituencyId);
            vm.UrbanRuralName = await c.GetNameAsync(() => vm.Establishment.UrbanRuralId);
            vm.GSSLAName = await c.GetNameAsync(() => vm.Establishment.GSSLAId);
            vm.MSOAName = await c.GetNameAsync(() => vm.Establishment.MSOAId);
            vm.LSOAName = await c.GetNameAsync(() => vm.Establishment.LSOAId);
            vm.HeadTitleName = await c.GetNameAsync(() => vm.Establishment.HeadTitleId);
            vm.EducationPhaseName = await c.GetNameAsync(() => vm.Establishment.EducationPhaseId);
            vm.TypeName = await c.GetNameAsync(() => vm.Establishment.TypeId);
            vm.FurtherEducationTypeName = await c.GetNameAsync(() => vm.Establishment.FurtherEducationTypeId);
            vm.GenderName = await c.GetNameAsync(() => vm.Establishment.GenderId);
            vm.StatusName = await c.GetNameAsync(() => vm.Establishment.StatusId);
            vm.AdmissionsPolicyName = await c.GetNameAsync(() => vm.Establishment.AdmissionsPolicyId);
            vm.OfstedRatingName = await c.GetNameAsync(() => vm.Establishment.OfstedRatingId);
            vm.HelpdeskPreviousLocalAuthorityName = await c.GetNameAsync("LocalAuthorityId", vm.Establishment.HelpdeskPreviousLocalAuthorityId);

            var sens = await c.SpecialEducationNeedsGetAllAsync();
            vm.SENNames = StringUtil.SentencifyNoFormating((vm.Establishment.SENIds ?? new int[0]).Select(x => sens.FirstOrDefault(s => s.Id == x)?.Name).ToArray());

            vm.AddressCountryName = await c.GetNameAsync("CountryId", vm.Establishment.Address_CountryId);
            vm.AddressCountyName = await c.GetNameAsync("CountyId", vm.Establishment.Address_CountyId);

            if (vm.Establishment.AdditionalAddresses != null)
            {
                vm.AdditionalAddressList = await Task.WhenAll(vm.Establishment.AdditionalAddresses.Select(async x => new AdditionalAddressViewModel
                {
                    Address3 = x.Address3,
                    CountryId = x.CountryId,
                    Id = x.Id,
                    Locality = x.Locality,
                    Location = x.Location,
                    CountyId = x.CountyId,
                    PostCode = x.PostCode,
                    SiteName = x.SiteName,
                    Street = x.Street,
                    Town = x.Town,
                    UPRN = x.UPRN,
                    CountryName = (await c.GetNameAsync(() => x.CountryId)).RemoveSubstring("Not recorded"),
                    CountyName = (await c.GetNameAsync(() => x.CountyId)).RemoveSubstring("Not recorded")
                }));
            }

            if (vm.Establishment.LocalAuthorityId.HasValue)
            {
                var las = await c.LocalAuthorityGetAllAsync();
                var la = las.FirstOrDefault(x => x.Id == vm.Establishment.LocalAuthorityId);
                vm.LocalAuthorityName = la?.Name;
                vm.LocalAuthorityCode = la?.Code;
            }
        }

        private async Task PopulateSelectLists(ViewModel viewModel)
        {
            var localAuthorities = await _cachedLookupService.LocalAuthorityGetAllAsync();

            viewModel.AccommodationChanges = (await _cachedLookupService.AccommodationChangedGetAllAsync()).ToSelectList(viewModel.AccommodationChangedId);
            viewModel.FurtherEducationTypes = (await _cachedLookupService.FurtherEducationTypesGetAllAsync()).ToSelectList(viewModel.FurtherEducationTypeId);
            viewModel.Genders = (await _cachedLookupService.GendersGetAllAsync()).ToSelectList(viewModel.GenderId);
            viewModel.LocalAuthorities = localAuthorities.ToSelectList(viewModel.LocalAuthorityId);
            viewModel.EstablishmentTypes = (await _cachedLookupService.EstablishmentTypesGetAllAsync()).ToSelectList(viewModel.TypeId);
            viewModel.HeadTitles = (await _cachedLookupService.TitlesGetAllAsync()).ToSelectList(viewModel.HeadTitleId);
            viewModel.Statuses = (await _lookupService.EstablishmentStatusesGetAllAsync(User)).ToSelectList(viewModel.StatusId);
            viewModel.AdmissionsPolicies = (await _cachedLookupService.AdmissionsPoliciesGetAllAsync()).ToSelectList(viewModel.AdmissionsPolicyId);
            viewModel.Inspectorates = (await _cachedLookupService.InspectoratesGetAllAsync()).ToSelectList(viewModel.InspectorateId);
            viewModel.IndependentSchoolTypes = (await _cachedLookupService.IndependentSchoolTypesGetAllAsync()).ToSelectList(viewModel.IndependentSchoolTypeId);
            viewModel.BSOInspectorates = (await _cachedLookupService.InspectorateNamesGetAllAsync()).ToSelectList(viewModel.BSOInspectorateId);
            viewModel.ReligiousCharacters = (await _cachedLookupService.ReligiousCharactersGetAllAsync()).ToSelectList(viewModel.ReligiousCharacterId);
            viewModel.ReligiousEthoses = (await _cachedLookupService.ReligiousEthosGetAllAsync()).ToSelectList(viewModel.ReligiousEthosId);
            viewModel.Dioceses = (await _cachedLookupService.DiocesesGetAllAsync()).ToSelectList(viewModel.DioceseId);
            viewModel.BoardingProvisions = (await _cachedLookupService.ProvisionBoardingGetAllAsync()).ToSelectList(viewModel.ProvisionBoardingId);
            viewModel.BoardingEstablishment = (await _cachedLookupService.BoardingEstablishmentGetAllAsync()).ToSelectList(viewModel.BoardingEstablishmentId);
            viewModel.QualityAssuranceBodyName = (await _cachedLookupService.QualityAssuranceBodyNameGetAllAsync()).ToSelectList(viewModel.QualityAssuranceBodyNameId);
            viewModel.EstablishmentAccredited = (await _cachedLookupService.EstablishmentAccreditedGetAllAsync()).ToSelectList(viewModel.EstablishmentAccreditedId);
            viewModel.NurseryProvisions = (await _cachedLookupService.ProvisionNurseriesGetAllAsync()).ToSelectList(viewModel.ProvisionNurseryId);
            viewModel.OfficialSixthFormProvisions = (await _cachedLookupService.ProvisionOfficialSixthFormsGetAllAsync()).ToSelectList(viewModel.ProvisionOfficialSixthFormId);
            viewModel.Section41ApprovedItems = (await _cachedLookupService.Section41ApprovedGetAllAsync()).ToSelectList(viewModel.Section41ApprovedId);
            viewModel.EducationPhases = (await _cachedLookupService.EducationPhasesGetAllAsync()).ToSelectList(viewModel.EducationPhaseId);
            viewModel.ReasonsEstablishmentOpened = (await _cachedLookupService.ReasonEstablishmentOpenedGetAllAsync()).ToSelectList(viewModel.ReasonEstablishmentOpenedId);
            viewModel.ReasonsEstablishmentClosed = (await _cachedLookupService.ReasonEstablishmentClosedGetAllAsync()).ToSelectList(viewModel.ReasonEstablishmentClosedId);
            viewModel.SpecialClassesProvisions = (await _cachedLookupService.ProvisionSpecialClassesGetAllAsync()).ToSelectList(viewModel.ProvisionSpecialClassesId);

            viewModel.MSOAs = (await _cachedLookupService.MSOAsGetAllAsync()).Select(x => new LookupItemViewModel(x.Id, $"{x.Name} [{x.Code}]")).ToList();
            viewModel.LSOAs = (await _cachedLookupService.LSOAsGetAllAsync()).Select(x => new LookupItemViewModel(x.Id, $"{x.Name} [{x.Code}]")).ToList();

            viewModel.SENProvisions = (await _cachedLookupService.SpecialEducationNeedsGetAllAsync()).ToList();

            viewModel.TypeOfResourcedProvisions = (await _cachedLookupService.TypeOfResourcedProvisionsGetAllAsync()).ToSelectList(viewModel.TypeOfResourcedProvisionId);
            viewModel.TeenageMothersProvisions = (await _cachedLookupService.TeenageMothersProvisionsGetAllAsync()).ToSelectList(viewModel.TeenageMothersProvisionId);
            viewModel.ChildcareFacilitiesProvisions = (await _cachedLookupService.ChildcareFacilitiesGetAllAsync()).ToSelectList(viewModel.ChildcareFacilitiesId);
            viewModel.RSCRegions = (await _cachedLookupService.RscRegionsGetAllAsync()).ToSelectList(viewModel.RSCRegionId);
            viewModel.GovernmentOfficeRegions = (await _cachedLookupService.GovernmentOfficeRegionsGetAllAsync()).ToSelectList(viewModel.GovernmentOfficeRegionId);
            viewModel.AdministrativeDistricts = (await _cachedLookupService.AdministrativeDistrictsGetAllAsync()).Select(x => new LookupItemViewModel(x.Id, x.Name)).ToList();
            viewModel.AdministrativeWards = (await _cachedLookupService.AdministrativeWardsGetAllAsync()).Select(x => new LookupItemViewModel(x.Id, $"{x.Name} [{x.Code}]")).ToList();
            viewModel.ParliamentaryConstituencies = (await _cachedLookupService.ParliamentaryConstituenciesGetAllAsync()).Select(x => new LookupItemViewModel(x.Id, x.Name)).ToList();
            viewModel.UrbanRuralLookup = (await _cachedLookupService.UrbanRuralGetAllAsync()).ToSelectList(viewModel.UrbanRuralId);
            viewModel.GSSLALookup = (await _cachedLookupService.GSSLAGetAllAsync()).Select(x => new LookupItemViewModel(x.Id, x.Name)).ToList();
            viewModel.PruFulltimeProvisionOptions = (await _cachedLookupService.PruFulltimeProvisionsGetAllAsync()).ToSelectList(viewModel.PruFulltimeProvisionId);
            viewModel.PruEducatedByOthersOptions = (await _cachedLookupService.PruEducatedByOthersGetAllAsync()).ToSelectList(viewModel.PruEducatedByOthersId);
            viewModel.PRUEBDOptions = (await _cachedLookupService.PRUEBDsGetAllAsync()).ToSelectList(viewModel.PRUEBDId);
            viewModel.PRUSENOptions = (await _cachedLookupService.PRUSENsGetAllAsync()).ToSelectList(viewModel.PRUSENId);

            viewModel.Counties = (await _cachedLookupService.CountiesGetAllAsync()).ToSelectList(viewModel.Address_CountyId);

            foreach (var viewModelProprietor in viewModel.Proprietors)
            {
                viewModelProprietor.Counties = (await _cachedLookupService.CountiesGetAllAsync()).ToSelectList(viewModelProprietor.CountyId);
            }

            if (viewModel.ChairOfProprietor != null)
            {
                viewModel.ChairOfProprietor.Counties = (await _cachedLookupService.CountiesGetAllAsync()).ToSelectList(viewModel.ChairOfProprietor.CountyId);
                if ((await _cachedLookupService.CountiesGetAllAsync()).Any(x => x.CodeAsInt != null))
                {
                    viewModel.ChairOfProprietor.CountyIdDefault = (await _cachedLookupService.CountiesGetAllAsync()).FirstOrDefault(x => x.Code.Equals("099"))?.Id; // Not recorded
                }
            }

            viewModel.Countries = (await _cachedLookupService.NationalitiesGetAllAsync()).ToSelectList(viewModel.Address_CountryId);
            viewModel.OfstedRatings = (await _cachedLookupService.OfstedRatingsGetAllAsync()).ToSelectList(viewModel.OfstedRatingId);

            viewModel.Type2PhaseMap = _establishmentReadService.GetEstabType2EducationPhaseMap().AsInts();
            viewModel.TypeName = await _cachedLookupService.GetNameAsync(() => viewModel.TypeId);
            viewModel.LegalParentGroup = GetLegalParent(viewModel.Urn.Value, await _groupReadService.GetAllByEstablishmentUrnAsync(viewModel.Urn.Value, User), User);

            viewModel.CCOperationalHours = (await _cachedLookupService.CCOperationalHoursGetAllAsync()).ToSelectList(viewModel.CCOperationalHoursId);
            viewModel.CCGovernanceList = (await _cachedLookupService.CCGovernanceGetAllAsync()).ToSelectList(viewModel.CCGovernanceId);
            viewModel.CCDeliveryModels = (await _cachedLookupService.CCDeliveryModelsGetAllAsync()).ToSelectList(viewModel.CCDeliveryModelId);
            viewModel.CCGroupLead = (await _cachedLookupService.CCGroupLeadsGetAllAsync()).ToSelectList(viewModel.CCGroupLeadId);
            viewModel.CCPhaseTypes = (await _cachedLookupService.CCPhaseTypesGetAllAsync()).ToSelectList(viewModel.CCPhaseTypeId);
            viewModel.CCDisadvantagedAreas = (await _cachedLookupService.CCDisadvantagedAreasGetAllAsync()).ToSelectList(viewModel.CCDisadvantagedAreaId);
            viewModel.CCDirectProvisionOfEarlyYears = (await _cachedLookupService.DirectProvisionOfEarlyYearsGetAllAsync()).ToSelectList(viewModel.CCDirectProvisionOfEarlyYearsId);

            if(viewModel.LocalAuthorityId.HasValue)
            {
                viewModel.LocalAuthorityCode = (from la in localAuthorities
                                                where la.Id == viewModel.LocalAuthorityId.Value
                                                select la.Code).FirstOrDefault();
            }
        }

        private async Task PopulateSelectLists(CreateEstablishmentViewModel viewModel)
        {
            viewModel.LocalAuthorities = (await _cachedLookupService.LocalAuthorityGetAllAsync()).ToSelectList(viewModel.LocalAuthorityId);
            viewModel.EstablishmentTypes = (await _cachedLookupService.EstablishmentTypesGetAllAsync()).Where(x => viewModel.CreateEstablishmentPermission.Types.Cast<int>().Contains(x.Id)).ToSelectList(viewModel.EstablishmentTypeId);
            viewModel.EducationPhases = (await _cachedLookupService.EducationPhasesGetAllAsync()).ToSelectList(viewModel.EducationPhaseId);
        }

        private async Task MapFromViewModelToDomainModel(ViewModel viewModel, EstablishmentModel domainModel)
        {
            MapToDomainModel(viewModel, domainModel);
            MapToDomainModelIEBT(viewModel, domainModel);

            if (_formKeys.Value.Contains(nameof(viewModel.MSOAName)) && _formKeys.Value.Contains(nameof(viewModel.MSOAId)))
            {
                // in case js is disabled and the ID field is empty, we still want to try to find the id from the lookup if possible.
                domainModel.MSOAId = viewModel.MSOAId ?? (await _cachedLookupService.MSOAsGetAllAsync()).FirstOrDefault(x => x.Name.Equals(viewModel.MSOAName, StringComparison.InvariantCultureIgnoreCase))?.Id;
            }

            if (_formKeys.Value.Contains(nameof(viewModel.LSOAName)) && _formKeys.Value.Contains(nameof(viewModel.LSOAId)))
            {
                // in case js is disabled and the ID field is empty, we still want to try to find the id from the lookup if possible.
                domainModel.LSOAId = viewModel.LSOAId ?? (await _cachedLookupService.LSOAsGetAllAsync()).FirstOrDefault(x => x.Name.Equals(viewModel.LSOAName, StringComparison.InvariantCultureIgnoreCase))?.Id;
            }
        }

        private void AddOrReplaceAddressFromUrlToken(string addrtok, ViewModel viewModel)
        {
            var addressViewModel = UriHelper.TryDeserializeUrlToken<AddOrReplaceAddressViewModel>(addrtok);
            if (addressViewModel != null)
            {
                if (addressViewModel.Target == "main")
                {
                    viewModel.Address_CityOrTown = addressViewModel.Town;
                    viewModel.Address_CountryId = addressViewModel.CountryId;
                    viewModel.Address_CountyId = addressViewModel.CountyId;
                    viewModel.Address_Line1 = addressViewModel.Street;
                    viewModel.Address_Locality = addressViewModel.Locality;
                    viewModel.Address_Line3 = addressViewModel.Address3;
                    viewModel.Address_PostCode = addressViewModel.PostCode;
                    viewModel.Address_UPRN = addressViewModel.SelectedUPRN;
                    viewModel.Northing = addressViewModel.Northing;
                    viewModel.Easting = addressViewModel.Easting;
                }
                else if (addressViewModel.Target.StartsWith("alt"))
                {
                    AdditionalAddressModel map(AddOrReplaceAddressViewModel source, AdditionalAddressModel dest)
                    {
                        dest.Town = source.Town;
                        dest.CountryId = source.CountryId;
                        dest.CountyId = source.CountyId;
                        dest.Street = source.Street;
                        dest.Locality = source.Locality;
                        dest.Address3 = source.Address3;
                        dest.PostCode = source.PostCode;
                        dest.UPRN = source.SelectedUPRN;
                        return dest;
                    };

                    var p = addressViewModel.Target.GetPart("-", 1);
                    var index = p.ToInteger().GetValueOrDefault();
                    if (index > (viewModel.AdditionalAddresses.Count - 1) || p == "new")
                    {
                        viewModel.AdditionalAddresses.Add(map(addressViewModel, new AdditionalAddressModel()));
                    }
                    else
                    {
                        map(addressViewModel, viewModel.AdditionalAddresses[index]);
                    }
                }
                viewModel.IsDirty = true;
            }
        }

        private async Task<ActionResult> SaveEstablishment(ViewModel viewModel)
        {
            var domainModel = (await _establishmentReadService.GetAsync(viewModel.Urn.Value, User)).GetResult();
            var editPolicyEnvelope = await _establishmentReadService.GetEditPolicyAsync(domainModel, User);
            var displayPolicyEnvelope = await _establishmentReadService.GetDisplayPolicyAsync(domainModel, User);
            viewModel.DisplayPolicy = displayPolicyEnvelope;
            viewModel.EditPolicy = editPolicyEnvelope.EditPolicy;
            viewModel.EmptyEmailFields = new List<string>();

            var canEditAdditionalAddresses = editPolicyEnvelope.EditPolicy.AdditionalAddresses;
            if (domainModel.AdditionalAddresses != null &&
                (viewModel.SelectedTab != "details" || !canEditAdditionalAddresses))
            {
                // the additional addresses can only be edited from within the details tab, so default copy from domain for any other tab edits
                viewModel.AdditionalAddresses = new List<AdditionalAddressModel>(domainModel.AdditionalAddresses);
            }

            viewModel.TabDisplayPolicy = new TabDisplayPolicy(domainModel, viewModel.EditPolicy, User);
            viewModel.CanOverrideCRProcess = User.IsInRole(AuthorizedRoles.IsAdmin);

            await PopulateSelectLists(viewModel);

            if (viewModel.ActionSpecifierCommand == ViewModel.ASConfirm)
            {
                if (ModelState.IsValid)
                {
                    return await SaveEstablishment(viewModel, domainModel);
                }
            }

            if (viewModel.ActionSpecifierCommand == ViewModel.ASEmailBack)
            {
                viewModel.IsUpdatingEmailFields = false;
                return View(viewModel);
            }

            if (viewModel.ActionSpecifierCommand == ViewModel.ASSave || viewModel.ActionSpecifierCommand == ViewModel.ASConfirm ||
                viewModel.ActionSpecifierCommand == ViewModel.ASSaveEmails || viewModel.ActionSpecifierCommand == ViewModel.ASAmendEmails)
            {
                var originalEstabTypeId = (ET) domainModel.TypeId;
                var originalName = domainModel.Name;

                await ValidateAsync(viewModel, domainModel, ModelState.IsValid || viewModel.ActionSpecifierCommand == ViewModel.ASConfirm);

                if (viewModel.ActionSpecifierCommand == ViewModel.ASAmendEmails && viewModel.IsUpdatingEmailFields.GetValueOrDefault())
                {
                    viewModel.HasEmptyEmailFields = true;
                }

                if (viewModel.ActionSpecifierCommand == ViewModel.ASSaveEmails)
                {
                    viewModel.IsUpdatingEmailFields = !ModelState.IsValid;
                    viewModel.HasEmptyEmailFields = !ModelState.IsValid;
                }

                if (((viewModel.EditPolicy.Contact_EmailAddress && viewModel.Contact_EmailAddress == null) ||
                     (viewModel.EditPolicy.HeadEmailAddress && viewModel.HeadEmailAddress == null)) && viewModel.ActionSpecifierCommand != ViewModel.ASSaveEmails)
                {
                    viewModel.HasEmptyEmailFields = true;

                    if ((viewModel.HeadEmailAddress == null || !ViewData.ModelState.IsValidField("HeadEmailAddress"))
                        && viewModel.EditPolicy.HeadEmailAddress)
                    {
                        viewModel.EmptyEmailFields.Add("HeadEmailAddress");
                    }

                    if ((viewModel.Contact_EmailAddress == null || !ViewData.ModelState.IsValidField("Contact_EmailAddress"))
                        && viewModel.EditPolicy.Contact_EmailAddress)
                    {
                        viewModel.EmptyEmailFields.Add("Contact_EmailAddress");
                    }

                    if ((viewModel.ContactAlt_EmailAddress == null || !ViewData.ModelState.IsValidField("ContactAlt_EmailAddress"))
                        && viewModel.EditPolicy.ContactAlt_EmailAddress)
                    {
                        viewModel.EmptyEmailFields.Add("ContactAlt_EmailAddress");
                    }
                }

                if (viewModel.ActionSpecifierCommand == ViewModel.ASAmendEmails && !viewModel.IsUpdatingEmailFields.GetValueOrDefault())
                {
                    viewModel.HasEmptyEmailFields = false;
                }

                var newEstabTypeId = (ET?) domainModel.TypeId;

                if (ModelState.IsValid || viewModel.ActionSpecifierCommand == ViewModel.ASConfirm)
                {
                    viewModel.OriginalEstablishmentName = originalName;

                    var changes = await _establishmentReadService.GetModelChangesAsync(domainModel, editPolicyEnvelope.ApprovalsPolicy, User);

                    if (originalEstabTypeId == ET.ChildrensCentreLinkedSite && newEstabTypeId == ET.ChildrensCentre)
                    {
                        viewModel.CCIsPromoting = true;
                    }
                    else if (originalEstabTypeId == ET.ChildrensCentre && newEstabTypeId == ET.ChildrensCentreLinkedSite)
                    {
                        viewModel.CCIsDemoting = true;
                    }

                    if (changes.Any())
                    {
                        viewModel.ChangesSummary = changes;
                        viewModel.ChangesRequireApprovalCount = changes.Count(x => x.RequiresApproval);
                        viewModel.ChangesInstantCount = changes.Count(x => !x.RequiresApproval);
                        ModelState.Remove(nameof(viewModel.ChangesRequireApprovalCount));
                        ModelState.Remove(nameof(viewModel.ChangesInstantCount));
                    }
                    else
                    {
                        return View("EditDetailsEmpty", viewModel);
                    }
                }
                else
                {
                    viewModel.OriginalEstablishmentName = originalName;
                    viewModel.OriginalTypeName = (await _cachedLookupService.EstablishmentTypesGetAllAsync()).Where(x => x.Id == (int) originalEstabTypeId).Select(x => x.Name).FirstOrDefault();
                    if (viewModel.IsUpdatingEmailFields.GetValueOrDefault())
                    {
                        var changes = await _establishmentReadService.GetModelChangesAsync(domainModel, editPolicyEnvelope.ApprovalsPolicy, User);
                        viewModel.ChangesSummary = changes;
                    }
                }
            }
            else if (viewModel.ActionSpecifierCommand == ViewModel.ASAddAddress)
            {
                viewModel.AdditionalAddresses.Add(new AdditionalAddressModel());
            }
            else if (viewModel.ActionSpecifierCommand == ViewModel.ASRemoveAddress)
            {
                RemoveAdditionalAddress(viewModel);
            }
            else if (viewModel.ActionSpecifierCommand == ViewModel.ASCancel)
            {
                viewModel.OriginalEstablishmentName = domainModel.Name;
                viewModel.OriginalTypeName = (await _cachedLookupService.EstablishmentTypesGetAllAsync()).Where(x => x.Id == domainModel.TypeId).Select(x => x.Name).FirstOrDefault();
            }
            else if (viewModel.ActionSpecifierCommand == ViewModel.ASUpdateProprietors)
            {
                if (viewModel.ProprietorTypeId.OneOfThese(eProprietorType.ProprietorBody))
                {
                    // if going from multiple proprietors to a single proprietor, we need to remove all subsequent proprietors
                    if (viewModel.Proprietors.Count > 1)
                    {
                        viewModel.Proprietors.RemoveRange(1, viewModel.Proprietors.Count - 1);
                        viewModel.IsDirty = true;
                    }
                }
            }
            else if (viewModel.ActionSpecifierCommand == ViewModel.ASAddProprietor)
            {
                viewModel.Proprietors.Add(new ProprietorViewModel
                {
                    Counties = (await _cachedLookupService.CountiesGetAllAsync()).ToSelectList(),
                });
                viewModel.IsDirty = true;
            }
            else if (viewModel.ActionSpecifierCommand == ViewModel.ASRemoveProprietor)
            {
                viewModel.Proprietors.RemoveAt(int.Parse(viewModel.ActionSpecifierParam) - 1);
                viewModel.IsDirty = true;
            }
            return View(viewModel);
        }

        private async Task<ActionResult> SaveEstablishment(ViewModel model, EstablishmentModel domainModel)
        {
            await MapFromViewModelToDomainModel(model, domainModel);

            await _establishmentWriteService.SaveAsync(domainModel, model.OverrideCRProcess,
                model.ChangeEffectiveDate.ToDateTime(), User);
            return Redirect(Url.RouteUrl("EstabDetails", new
            {
                id = model.Urn.Value,
                approved = model.GetChangesNotRequiringApprovalCount(),
                pending = model.GetChangesRequiringApprovalCount()
            }) + model.SelectedTab2DetailPageTabNameMapping[model.SelectedTab]);
        }

        private static void RemoveAdditionalAddress(ViewModel model)
        {
            model.AdditionalAddresses.RemoveAt(int.Parse(model.ActionSpecifierParam));
            model.IsDirty = true;
        }

        private void SetProperty<TProperty>(ViewModel oldModel, ViewModel newModel, Expression<Func<ViewModel, TProperty>> property)
        {
            if (property.Body is MemberExpression memberExpression)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    var propertyName = propertyInfo.Name;
                    var policyGetter = typeof(EstablishmentDisplayEditPolicy).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)?.GetGetMethod();
                    if (policyGetter == null || (bool) policyGetter.Invoke(oldModel.EditPolicy, null))
                    {
                        propertyInfo.SetValue(oldModel, propertyInfo.GetValue(newModel));
                    }
                }
            }
        }

        /// <summary>
        /// Does 2nd-level validation
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="existingDomainModel"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        private async Task ValidateAsync(ViewModel viewModel, EstablishmentModel existingDomainModel, bool validate)
        {
            if (validate)
            {
                await MapFromViewModelToDomainModel(viewModel, existingDomainModel);

                var validationEnvelope = await _establishmentWriteService.ValidateAsync(existingDomainModel, User);

                viewModel.ShowDuplicateRecordError = validationEnvelope.Errors.Any(x => x.Code == "establishment.edit.with.same.name.la.postcode.found");

                if (viewModel.AccreditationExpiryDate.Year != null)
                {
                    var AccreditationEnd = new DateTime((int) viewModel.AccreditationExpiryDate.Year, (int) viewModel.AccreditationExpiryDate.Month, (int) viewModel.AccreditationExpiryDate.Day);
                    var AccreditationStart = new DateTime((int) viewModel.OpenDate.Year, (int) viewModel.OpenDate.Month, (int) viewModel.OpenDate.Day);

                    if (AccreditationEnd < AccreditationStart)
                    {
                        ModelState.AddModelError(nameof(viewModel.AccreditationExpiryDate), "Accreditation expiry date must be before accreditation start date");
                    }
                }

                if (viewModel.ShowDuplicateRecordError)
                {
                    ModelState.AddModelError(nameof(viewModel.Name), "Please enter a different establishment name");
                    ModelState.AddModelError(nameof(viewModel.LocalAuthorityId), "Please enter a different local authority");
                    ModelState.AddModelError(nameof(viewModel.Address_PostCode), "Please enter a different postcode");
                }
                else
                {
                    foreach (var validationEnvelopeError in validationEnvelope.Errors)
                    {
                        var fieldName = validationEnvelopeError.Fields;
                        if (fieldName != null)
                        {
                            if (fieldName.Contains(".") &&
                                ModelState.ContainsKey(fieldName.Split('.')[1]))
                            {
                                fieldName = fieldName.Split('.')[1];
                            }
                        }

                        if (fieldName == null || !ModelState.ContainsKey(fieldName) || !ModelState[fieldName].Errors.Any())
                        {
                            ModelState.AddModelError(fieldName ?? string.Empty, validationEnvelopeError.GetMessage());
                        }

                    }
                }

                viewModel.ShowDuplicateRecordWarning = validationEnvelope.HasWarnings && validationEnvelope.Warnings.Any(x => x.Code == "establishment.with.same.name.la.found");
            }
        }
    }
}
