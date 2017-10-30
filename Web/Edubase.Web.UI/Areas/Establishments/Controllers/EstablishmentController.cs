﻿using AutoMapper;
using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Services;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
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
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Validation;
using FluentValidation.Mvc;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Services.Establishments.DisplayPolicies;
using ViewModel = Edubase.Web.UI.Models.EditEstablishmentModel;

namespace Edubase.Web.UI.Areas.Establishments.Controllers
{
    [RouteArea("Establishments"), RoutePrefix("Establishment")]
    public class EstablishmentController : EduBaseController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;
        private readonly IMapper _mapper;
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly ICachedLookupService _cachedLookupService;
        private readonly IResourcesHelper _resourcesHelper;
        private readonly ISecurityService _securityService;

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
            ISecurityService securityService)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _mapper = mapper;
            _establishmentWriteService = establishmentWriteService;
            _cachedLookupService = cachedLookupService;
            _resourcesHelper = resourcesHelper;
            _securityService = securityService;
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}", Name = "EditEstablishmentDetail")]
        public async Task<ActionResult> EditDetails(int? id, string addrtok)
        {
            if (!id.HasValue) return HttpNotFound();
            ViewModel viewModel = await CreateEditViewModel(id, vm =>
            {
                if (addrtok.Clean() != null)
                    ReplaceAddressFromUrlToken(addrtok, vm);
            });
            viewModel.SelectedTab = "details";
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}")]
        public async Task<ActionResult> EditDetails(ViewModel model)
        {
            return await SaveEstablishment(model);
        }


        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/Helpdesk")]
        public async Task<ActionResult> EditHelpdesk(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var viewModel = await CreateEditViewModel(id);
            if (!viewModel.TabDisplayPolicy.Helpdesk) throw new PermissionDeniedException();
            viewModel.SelectedTab = "helpdesk";
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}/Helpdesk")]
        public async Task<ActionResult> EditHelpdesk(ViewModel model)
        {
            return await SaveEstablishment(model);
        }

        
        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/Location")]
        public async Task<ActionResult> EditLocation(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var viewModel = await CreateEditViewModel(id);
            if (!viewModel.TabDisplayPolicy.Location) throw new PermissionDeniedException();
            viewModel.SelectedTab = "location";
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}/Location")]
        public async Task<ActionResult> EditLocation(ViewModel model)
        {
            var oldModel = await CreateEditViewModel(model.Urn);

            SetProperty(oldModel, model, m => m.RSCRegionId);
            SetProperty(oldModel, model, m => m.GovernmentOfficeRegionId);
            SetProperty(oldModel, model, m => m.AdministrativeDistrictId);
            SetProperty(oldModel, model, m => m.AdministrativeWardId);
            SetProperty(oldModel, model, m => m.ParliamentaryConstituencyId);
            SetProperty(oldModel, model, m => m.UrbanRuralId);
            SetProperty(oldModel, model, m => m.GSSLAId);
            SetProperty(oldModel, model, m => m.Easting);
            SetProperty(oldModel, model, m => m.Northing);
            SetProperty(oldModel, model, m => m.CASWardId);
            SetProperty(oldModel, model, m => m.MSOAId);
            SetProperty(oldModel, model, m => m.LSOAId);

            oldModel.Action = model.Action;
            oldModel.SelectedTab = model.SelectedTab;
            oldModel.OverrideCRProcess = model.OverrideCRProcess;
            oldModel.ChangeEffectiveDate = model.ChangeEffectiveDate;

            return await SaveEstablishment(oldModel);
        }

        private void SetProperty<TProperty>(ViewModel oldModel, ViewModel newModel, Expression<Func<ViewModel, TProperty>> property)
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    var propertyName = propertyInfo.Name;
                    var policyGetter = typeof(EstablishmentDisplayEditPolicy).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)?.GetGetMethod();
                    if (policyGetter == null || (bool)policyGetter.Invoke(oldModel.EditPolicy, null))
                    {
                        propertyInfo.SetValue(oldModel, propertyInfo.GetValue(newModel));
                    }
                }
            }
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/IEBT")]
        public async Task<ActionResult> EditIEBT(int? id)
        {
            if (!id.HasValue) return HttpNotFound();
            var viewModel = await CreateEditViewModel(id);
            if (!viewModel.TabDisplayPolicy.IEBT) throw new PermissionDeniedException();
            viewModel.SelectedTab = "iebt";
            return View("EditIEBT", viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize, Route("Edit/{id:int}/IEBT")]
        public async Task<ActionResult> EditIEBT(ViewModel model)
        {
            return await SaveEstablishment(model);
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/Links/Search", Name = "EditEstabLinks_SearchForEstablishment")]
        public async Task<ActionResult> SearchForEstablishment(int? id, SearchForEstablishmentViewModel viewModel)
        {
            viewModel = viewModel ?? new SearchForEstablishmentViewModel();
            if (!id.HasValue) return HttpNotFound();
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

        [HttpGet, EdubaseAuthorize, Route("Edit/{urn:int}/Address/{target}", Name = "ReplaceEstablishmentAddress")]
        public async Task<ActionResult> ReplaceEstablishmentAddressAsync(int urn, string target)
        {
            var viewModel = new ReplaceAddressViewModel((await _cachedLookupService.NationalitiesGetAllAsync()).ToSelectList(Constants.COUNTRY_ID_UK), 
                (await _cachedLookupService.CountiesGetAllAsync()).ToSelectList(), target);
            await PopulateEstablishmentPageViewModel(viewModel, urn, "details");
            return View("ReplaceAddress", viewModel);
        }

        [HttpPost, EdubaseAuthorize, Route("Edit/{urn:int}/Address/{target}", Name = "ReplaceEstablishmentAddressPost")]
        public async Task<ActionResult> ReplaceEstablishmentAddressPostAsync(int urn, string target, ReplaceAddressViewModel viewModel)
        {
            ModelState.Clear();

            if(viewModel.ActionName == null)
            {
                if (viewModel.CountryId == Constants.COUNTRY_ID_UK) viewModel.Step = "enterpostcode";
                else viewModel.Step = "editaddress";
            }
            else if (viewModel.ActionName == "find-address")
            {
                if(viewModel.CountryId == Constants.COUNTRY_ID_UK)
                {
                    viewModel.LookupAddresses = await _establishmentReadService.GetAddressesByPostCodeAsync(viewModel.PostCode, User);
                    if(viewModel.LookupAddresses.Any()) viewModel.Step = "selectaddress";
                    else ModelState.AddModelError("PostCode", "We couldn't find any addresses matching that postcode");
                }
                else viewModel.Step = "editaddress";
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
            return View("ReplaceAddress", viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}/Links", Name = "EditEstabLinks")]
        public async Task<ActionResult> EditLinks(int? id, bool saved = false)
        {
            ViewBag.ShowSaved = saved;

            if (!id.HasValue) return HttpNotFound();

            var viewModel = new EditEstablishmentLinksViewModel();
            await PopulateEstablishmentPageViewModel(viewModel, id.Value, "links");
            viewModel.Links = (await _establishmentReadService.GetLinkedEstablishmentsAsync(id.Value, User)).Select(x => new LinkedEstabViewModel(x)).ToList();
            await Task.WhenAll(viewModel.Links.Select(async x => x.LinkTypeName = await _cachedLookupService.GetNameAsync(() => x.LinkTypeId)));

            return View("EditLinks", viewModel);
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
                viewModel.ActiveRecord.Address = await ((await _establishmentReadService.GetAsync(viewModel.ActiveRecord.Urn.Value, User)).GetResult()).GetAddressAsync(_cachedLookupService);
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
            if(deltaViewModel.Act == "delete") return await DeleteLinkAsync(deltaViewModel);
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

                    if (!link.Id.HasValue) set.Add(link);

                    var apiResponse = await _establishmentWriteService.SaveLinkedEstablishmentsAsync(deltaViewModel.Urn.Value, set.ToArray(), User);

                    if (apiResponse.HasErrors) apiResponse.Errors.ForEach(x => ModelState.AddModelError(x.Fields ?? string.Empty, x.GetMessage()));
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

        [HttpGet, Route("Details/{id:int}", Name = "EstabDetails")]
        public async Task<ActionResult> Details(int id, string searchQueryString = "", eLookupSearchSource searchSource = eLookupSearchSource.Establishments, bool approved = false, bool pendingApproval = false, int skip = 0, string sortBy = null, bool saved = false)
        {
            ViewBag.ShowApproved = approved;
            ViewBag.PendingApproval = pendingApproval;
            ViewBag.ShowSaved = saved;

            var viewModel = new EstablishmentDetailViewModel
            {
                IsUserLoggedOn = User.Identity.IsAuthenticated,
                SearchQueryString = searchQueryString,
                SearchSource = searchSource
            };

            var result = await _establishmentReadService.GetAsync(id, User);
            if (result.ReturnValue == null) return HttpNotFound();
            viewModel.Establishment = result.ReturnValue;
            
            await Task.WhenAll(
                PopulateLinkedEstablishments(id, viewModel),
                PopulateChangeHistory(id, viewModel, skip, 100, sortBy),
                PopulateGroups(id, viewModel),
                PopulateDisplayPolicies(viewModel),
                PopulateEditPermissions(viewModel),
                PopulateLookupNames(viewModel),
                PopulateGovernors(viewModel));

            viewModel.AgeRangeToolTip = _resourcesHelper.GetResourceStringForEstablishment("AgeRange", (eLookupEstablishmentTypeGroup?)viewModel.Establishment.EstablishmentTypeGroupId, User);
            viewModel.AgeRangeToolTipLink = _resourcesHelper.GetResourceStringForEstablishment("AgeRangeLink", (eLookupEstablishmentTypeGroup?)viewModel.Establishment.EstablishmentTypeGroupId, User);
            viewModel.SchoolCapacityToolTip = _resourcesHelper.GetResourceStringForEstablishment("SchoolCapacity", (eLookupEstablishmentTypeGroup?)viewModel.Establishment.EstablishmentTypeGroupId, User);
            viewModel.SchoolCapacityToolTipLink = _resourcesHelper.GetResourceStringForEstablishment("SchoolCapacityLink", (eLookupEstablishmentTypeGroup?)viewModel.Establishment.EstablishmentTypeGroupId, User);

            return View(viewModel);
        }

        [HttpGet, EdubaseAuthorize, Route("Create", Name = "CreateEstablishment")]
        public async Task<ActionResult> Create()
        {
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
        public async Task<ActionResult> Create(CreateChildrensCentreViewModel viewModel)
        {
            viewModel.CreateEstablishmentPermission = await _securityService.GetCreateEstablishmentPermissionAsync(User);
            viewModel.Type2PhaseMap = _establishmentReadService.GetEstabType2EducationPhaseMap().AsInts();

            if (viewModel.EstablishmentTypeId == 41)
                return await CreateChildrensCentre(viewModel);

            if (ModelState.IsValid)
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
                    StatusId = (int)eLookupEstablishmentStatus.ProposedToOpen
                };

                var validation = await _establishmentWriteService.ValidateCreateAsync(apiModel, true, User);

                ApplyCreateEstabValidationErrors(validation);

                if (ModelState.IsValid)
                {
                    viewModel.SetWarnings(validation);
                    ModelState.Remove(nameof(viewModel.ProcessedWarnings));
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

            await PopulateCCSelectLists(viewModel);

            return View(viewModel);
        }

        [HttpPost, EdubaseAuthorize, Route("Confirm/{urn:int}", Name = "EstablishmentConfirmUpToDate")]
        public async Task<ActionResult> EstablishmentConfirmUpToDateAsync(int urn)
        {
            await _establishmentWriteService.ConfirmAsync(urn, User);
            return RedirectToRoute("EstabDetails", new { id = urn });
        }

        private void ReplaceAddressFromUrlToken(string addrtok, ViewModel viewModel)
        {
            var replaceAddressViewModel = UriHelper.TryDeserializeUrlToken<ReplaceAddressViewModel>(addrtok);
            if (replaceAddressViewModel != null)
            {
                if (replaceAddressViewModel.Target == "main")
                {
                    viewModel.Address_CityOrTown = replaceAddressViewModel.Town;
                    viewModel.Address_CountryId = replaceAddressViewModel.CountryId;
                    viewModel.Address_CountyId = replaceAddressViewModel.CountyId;
                    viewModel.Address_Line1 = replaceAddressViewModel.Street;
                    viewModel.Address_Locality = replaceAddressViewModel.Locality;
                    viewModel.Address_Line3 = replaceAddressViewModel.Address3;
                    viewModel.Address_PostCode = replaceAddressViewModel.PostCode;
                    viewModel.Address_UPRN = replaceAddressViewModel.SelectedUPRN;
                    viewModel.Northing = replaceAddressViewModel.Northing;
                    viewModel.Easting = replaceAddressViewModel.Easting;
                }
                else if (replaceAddressViewModel.Target == "alt")
                {
                    viewModel.AltTown = replaceAddressViewModel.Town;
                    viewModel.AltCountryId = replaceAddressViewModel.CountryId;
                    viewModel.AltCountyId = replaceAddressViewModel.CountyId;
                    viewModel.AltStreet = replaceAddressViewModel.Street;
                    viewModel.AltLocality = replaceAddressViewModel.Locality;
                    viewModel.AltAddress3 = replaceAddressViewModel.Address3;
                    viewModel.AltPostCode = replaceAddressViewModel.PostCode;
                    viewModel.AltUPRN = replaceAddressViewModel.SelectedUPRN;
                }
                viewModel.IsDirty = true;
            }
        }

        private async Task<ActionResult> DeleteLinkAsync(EditEstablishmentLinksViewModel deltaViewModel)
        {
            await _establishmentWriteService.DeleteLinkedEstablishmentAsync(deltaViewModel.Urn.Value, deltaViewModel.ActiveRecord.Id.Value, User);
            var reverseLinks = await _establishmentReadService.GetLinkedEstablishmentsAsync(deltaViewModel.ActiveRecord.Urn.Value, User);

            var reverseLink = reverseLinks.FirstOrDefault(x => x.Urn == deltaViewModel.Urn);
            if (reverseLink != null)
                await _establishmentWriteService.DeleteLinkedEstablishmentAsync(deltaViewModel.ActiveRecord.Urn.Value, reverseLink.Id.Value, User);
            
            return RedirectToRoute("EditEstabLinks", new { id = deltaViewModel.Urn });
        }

        private async Task<ActionResult> SaveEstablishment(ViewModel model)
        {
            var domainModel = (await _establishmentReadService.GetAsync(model.Urn.Value, User)).GetResult();
            model.EditPolicy = await _establishmentReadService.GetEditPolicyAsync(domainModel, User);
            model.TabDisplayPolicy = new TabDisplayPolicy(domainModel, model.EditPolicy, User);
            model.CanOverrideCRProcess = User.IsInRole(EdubaseRoles.ROLE_BACKOFFICE);
            await PopulateSelectLists(model);

            if (model.Action == ViewModel.eAction.SaveDetails || model.Action == ViewModel.eAction.SaveIEBT || model.Action == ViewModel.eAction.SaveLocation)
            {
                var originalEstabTypeId = (eLookupEstablishmentType) domainModel.TypeId;
                await ValidateAsync(model, domainModel);
                var newEstabTypeId = (eLookupEstablishmentType?)domainModel.TypeId;

                if (ModelState.IsValid)
                {
                    model.OriginalEstablishmentName = domainModel.Name;
                    
                    var changes = await _establishmentReadService.GetModelChangesAsync(domainModel, User);

                    if (originalEstabTypeId == eLookupEstablishmentType.ChildrensCentreLinkedSite && newEstabTypeId == eLookupEstablishmentType.ChildrensCentre) model.CCIsPromoting = true;
                    else if (originalEstabTypeId == eLookupEstablishmentType.ChildrensCentre && newEstabTypeId == eLookupEstablishmentType.ChildrensCentreLinkedSite) model.CCIsDemoting = true;

                    if (changes.Any()) model.ChangesSummary = changes;
                    else return Redirect(Url.RouteUrl("EstabDetails", new { id = model.Urn.Value, approved = model.OverrideCRProcess }) + model.SelectedTab2DetailPageTabNameMapping[model.SelectedTab]);
                }
            }
            else if (model.Action == ViewModel.eAction.Confirm)
            {
                if (ModelState.IsValid)
                {
                    await PrepareModels(model, domainModel);
                    await _establishmentWriteService.SaveAsync(domainModel, model.OverrideCRProcess, model.ChangeEffectiveDate.ToDateTime(), User);
                    return Redirect(Url.RouteUrl("EstabDetails", new { id = model.Urn.Value, approved = model.OverrideCRProcess, pendingApproval = !model.OverrideCRProcess }) + model.SelectedTab2DetailPageTabNameMapping[model.SelectedTab]);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Does 2nd-level validation
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task ValidateAsync(ViewModel viewModel, EstablishmentModel existingDomainModel)
        {
            if (ModelState.IsValid)
            {
                await PrepareModels(viewModel, existingDomainModel);
                var validationEnvelope = await _establishmentWriteService.ValidateAsync(existingDomainModel, User);
                viewModel.ShowDuplicateRecordError = validationEnvelope.Errors.Any(x => x.Code == "establishment.edit.with.same.name.la.postcode.found");
                if (viewModel.ShowDuplicateRecordError)
                {
                    ModelState.AddModelError(nameof(viewModel.Name), "Please enter a different establishment name");
                    ModelState.AddModelError(nameof(viewModel.LocalAuthorityId), "Please enter a different local authority");
                    ModelState.AddModelError(nameof(viewModel.Address_PostCode), "Please enter a different postcode");
                }
                else validationEnvelope.Errors.ForEach(x => ModelState.AddModelError(x.Fields ?? string.Empty, x.GetMessage()));

                viewModel.ShowDuplicateRecordWarning = validationEnvelope.HasWarnings && validationEnvelope.Warnings.Any(x => x.Code == "establishment.with.same.name.la.found");
            }
        }

        private async Task PrepareModels(ViewModel model, EstablishmentModel domainModel)
        {
            model.LSOAId = (await _cachedLookupService.LSOAsGetAllAsync()).FirstOrDefault(x => x.Code == model.LSOACode)?.Id;
            model.MSOAId = (await _cachedLookupService.MSOAsGetAllAsync()).FirstOrDefault(x => x.Code == model.MSOACode)?.Id;
            
            MapToDomainModel(model, domainModel, Request.Form);
            MapToDomainModelIEBT(model, domainModel, Request.Form);
        }

        /// <summary>
        /// Transfers property values from the view model to the domain model
        /// where the value is passed into the form; i.e., the key is present.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="domainModel"></param>
        /// <param name="form"></param>
        private void MapToDomainModel(ViewModel viewModel, EstablishmentModel domainModel, NameValueCollection form)
        {
            var keys = form.AllKeys.Select(x => x.GetPart(".")).Distinct();

            var properties = ReflectionHelper.GetProperties(domainModel, writeableOnly: true);
            properties = properties.Where(x => keys.Contains(x)).ToList();

            var viewModelProperties = ReflectionHelper.GetProperties(viewModel);

            foreach (var item in properties.Intersect(viewModelProperties))
            {
                var info = ReflectionHelper.GetPropertyInfo(viewModel, item);
                if(info.Type == typeof(DateTimeViewModel))
                {
                    var value = ReflectionHelper.GetPropertyValue<DateTimeViewModel>(viewModel, item).ToDateTime()?.Date;
                    ReflectionHelper.SetProperty(domainModel, item, value);
                }
                else
                {
                    var value = ReflectionHelper.GetPropertyValue(viewModel, item);
                    ReflectionHelper.SetProperty(domainModel, item, value);
                }
            }

            if(keys.Contains("SENList")) domainModel.SENIds = viewModel.SENIds ?? new int[0];
            if (keys.Contains(nameof(viewModel.MSOACode))) domainModel.MSOAId = viewModel.MSOAId;
            if (keys.Contains(nameof(viewModel.LSOACode))) domainModel.LSOAId = viewModel.LSOAId;
        }

        private void MapToDomainModelIEBT(ViewModel viewModel, EstablishmentModel domainModel, NameValueCollection form)
        {
            var keys = form.AllKeys.Select(x => x.GetPart(".")).Distinct();

            var properties = ReflectionHelper.GetProperties(domainModel.IEBTModel, writeableOnly: true);
            properties = properties.Where(x => keys.Contains(x)).ToList();

            var viewModelProperties = ReflectionHelper.GetProperties(viewModel);

            foreach (var item in properties.Intersect(viewModelProperties))
            {
                var info = ReflectionHelper.GetPropertyInfo(viewModel, item);
                if (info.Type == typeof(DateTimeViewModel))
                {
                    var value = ReflectionHelper.GetPropertyValue<DateTimeViewModel>(viewModel, item).ToDateTime()?.Date;
                    ReflectionHelper.SetProperty(domainModel.IEBTModel, item, value);
                }
                else
                {
                    var value = ReflectionHelper.GetPropertyValue(viewModel, item);
                    ReflectionHelper.SetProperty(domainModel.IEBTModel, item, value);
                }
            }
        }

        private async Task<ViewModel> CreateEditViewModel(int? id, Action<ViewModel> preprocessViewModel = null)
        {
            var domainModel = (await _establishmentReadService.GetAsync(id.Value, User)).GetResult();
            var viewModel = _mapper.Map<ViewModel>(domainModel);
            _mapper.Map(domainModel.IEBTModel, viewModel);

            viewModel.EditPolicy = await _establishmentReadService.GetEditPolicyAsync(domainModel, User);
            viewModel.TabDisplayPolicy = new TabDisplayPolicy(domainModel, viewModel.EditPolicy, User);
            viewModel.CanOverrideCRProcess = User.IsInRole(EdubaseRoles.ROLE_BACKOFFICE);
            viewModel.SENIds = viewModel.SENIds ?? new int[0];

            preprocessViewModel?.Invoke(viewModel);

            await PopulateSelectLists(viewModel);
            return viewModel;
        }

        private async Task PopulateGovernors(EstablishmentDetailViewModel viewModel)
        {
            try
            {
                var governorsController = DependencyResolver.Current.GetService<GovernorController>();
                viewModel.GovernorsGridViewModel = await governorsController.CreateGovernorsViewModel(establishmentModel: viewModel.Establishment, user: User);
            }
            catch (Exception) // todo: tech debt, need to more gracefully handle 404 in this instance.
            {
                viewModel.GovernorsGridViewModel = new Governors.Models.GovernorsGridViewModel { DomainModel = new Services.Governors.Models.GovernorsDetailsDto() };
            }
        }

        private async Task PopulateEstablishmentPageViewModel(IEstablishmentPageViewModel viewModel, int urn, string selectedTabName)
        {
            viewModel.SelectedTab = selectedTabName;
            viewModel.Urn = urn;
            var domainModel = (await _establishmentReadService.GetAsync(urn, User)).GetResult();
            var editPolicy = await _establishmentReadService.GetEditPolicyAsync(domainModel, User);
            viewModel.TabDisplayPolicy = new TabDisplayPolicy(domainModel, editPolicy, User);
            viewModel.Name = domainModel.Name;
            if (domainModel.TypeId.HasValue) viewModel.TypeName = (await _cachedLookupService.GetNameAsync(() => domainModel.TypeId));
            viewModel.LegalParentGroup = GetLegalParent(viewModel.Urn.Value, await _groupReadService.GetAllByEstablishmentUrnAsync(viewModel.Urn.Value, User), User);
        }

        private async Task PopulateEditPermissions(EstablishmentDetailViewModel viewModel)
        {
            viewModel.UserCanEdit = await _establishmentReadService.CanEditAsync(viewModel.Establishment.Urn.Value, User);
        }

        private async Task PopulateDisplayPolicies(EstablishmentDetailViewModel viewModel)
        {
            viewModel.DisplayPolicy = await _establishmentReadService.GetDisplayPolicyAsync(viewModel.Establishment, User);
            viewModel.TabDisplayPolicy = new TabDisplayPolicy(viewModel.Establishment, viewModel.DisplayPolicy, User);
        }

        private async Task PopulateGroups(int id, EstablishmentDetailViewModel viewModel)
        {
            viewModel.Groups = await _groupReadService.GetAllByEstablishmentUrnAsync(id, User);
            viewModel.LegalParentGroup = GetLegalParent(id, viewModel.Groups, User);
        }

        private async Task PopulateChangeHistory(int id, EstablishmentDetailViewModel viewModel, int skip, int take, string sortBy)
        {
            if (User.Identity.IsAuthenticated)
            {
                viewModel.ChangeHistory = await _establishmentReadService.GetChangeHistoryAsync(id, skip, take, sortBy, User);
            }
        }

        private async Task PopulateLinkedEstablishments(int id, EstablishmentDetailViewModel viewModel)
        {
            viewModel.LinkedEstablishments = (await _establishmentReadService.GetLinkedEstablishmentsAsync(id, User)).Select(x => new LinkedEstabViewModel(x)).ToList();
            await Task.WhenAll(viewModel.LinkedEstablishments.Select(async x => x.LinkTypeName = await _cachedLookupService.GetNameAsync(() => x.LinkTypeId)));
        }

        private async Task PopulateSelectLists(ViewModel viewModel)
        {
            viewModel.AccommodationChanges = (await _cachedLookupService.AccommodationChangedGetAllAsync()).ToSelectList(viewModel.AccommodationChangedId);
            viewModel.FurtherEducationTypes = (await _cachedLookupService.FurtherEducationTypesGetAllAsync()).ToSelectList(viewModel.FurtherEducationTypeId);
            viewModel.Genders = (await _cachedLookupService.GendersGetAllAsync()).ToSelectList(viewModel.GenderId);
            viewModel.LocalAuthorities = (await _cachedLookupService.LocalAuthorityGetAllAsync()).ToSelectList(viewModel.LocalAuthorityId);
            viewModel.EstablishmentTypes = (await _cachedLookupService.EstablishmentTypesGetAllAsync()).ToSelectList(viewModel.TypeId);
            viewModel.HeadTitles = (await _cachedLookupService.TitlesGetAllAsync()).ToSelectList(viewModel.HeadTitleId);
            viewModel.Statuses = (await _cachedLookupService.EstablishmentStatusesGetAllAsync()).ToSelectList(viewModel.StatusId);
            viewModel.AdmissionsPolicies = (await _cachedLookupService.AdmissionsPoliciesGetAllAsync()).ToSelectList(viewModel.AdmissionsPolicyId);
            viewModel.Inspectorates = (await _cachedLookupService.InspectoratesGetAllAsync()).ToSelectList(viewModel.InspectorateId);
            viewModel.IndependentSchoolTypes = (await _cachedLookupService.IndependentSchoolTypesGetAllAsync()).ToSelectList(viewModel.IndependentSchoolTypeId);
            viewModel.BSOInspectorates = (await _cachedLookupService.InspectorateNamesGetAllAsync()).ToSelectList(viewModel.BSOInspectorateId);
            viewModel.ReligiousCharacters = (await _cachedLookupService.ReligiousCharactersGetAllAsync()).ToSelectList(viewModel.ReligiousCharacterId);
            viewModel.ReligiousEthoses = (await _cachedLookupService.ReligiousEthosGetAllAsync()).ToSelectList(viewModel.ReligiousEthosId);
            viewModel.Dioceses = (await _cachedLookupService.DiocesesGetAllAsync()).ToSelectList(viewModel.DioceseId);
            viewModel.BoardingProvisions = (await _cachedLookupService.ProvisionBoardingGetAllAsync()).ToSelectList(viewModel.ProvisionBoardingId);
            viewModel.BoardingEstablishment = (await _cachedLookupService.BoardingEstablishmentGetAllAsync()).ToSelectList(viewModel.BoardingEstablishmentId);
            viewModel.NurseryProvisions = (await _cachedLookupService.ProvisionNurseriesGetAllAsync()).ToSelectList(viewModel.ProvisionNurseryId);
            viewModel.OfficialSixthFormProvisions = (await _cachedLookupService.ProvisionOfficialSixthFormsGetAllAsync()).ToSelectList(viewModel.ProvisionOfficialSixthFormId);
            viewModel.Section41ApprovedItems = (await _cachedLookupService.Section41ApprovedGetAllAsync()).ToSelectList(viewModel.Section41ApprovedId);
            viewModel.EducationPhases = (await _cachedLookupService.EducationPhasesGetAllAsync()).ToSelectList(viewModel.EducationPhaseId);
            viewModel.ReasonsEstablishmentOpened = (await _cachedLookupService.ReasonEstablishmentOpenedGetAllAsync()).ToSelectList(viewModel.ReasonEstablishmentOpenedId);
            viewModel.ReasonsEstablishmentClosed = (await _cachedLookupService.ReasonEstablishmentClosedGetAllAsync()).ToSelectList(viewModel.ReasonEstablishmentClosedId);
            viewModel.SpecialClassesProvisions = (await _cachedLookupService.ProvisionSpecialClassesGetAllAsync()).ToSelectList(viewModel.ProvisionSpecialClassesId);

            viewModel.SENProvisions = (await _cachedLookupService.SpecialEducationNeedsGetAllAsync()).ToList();

            viewModel.TypeOfResourcedProvisions = (await _cachedLookupService.TypeOfResourcedProvisionsGetAllAsync()).ToSelectList(viewModel.TypeOfResourcedProvisionId);
            viewModel.TeenageMothersProvisions = (await _cachedLookupService.TeenageMothersProvisionsGetAllAsync()).ToSelectList(viewModel.TeenageMothersProvisionId);
            viewModel.ChildcareFacilitiesProvisions = (await _cachedLookupService.ChildcareFacilitiesGetAllAsync()).ToSelectList(viewModel.ChildcareFacilitiesId);
            viewModel.RSCRegions = (await _cachedLookupService.RscRegionsGetAllAsync()).ToSelectList(viewModel.RSCRegionId);
            viewModel.GovernmentOfficeRegions = (await _cachedLookupService.GovernmentOfficeRegionsGetAllAsync()).ToSelectList(viewModel.GovernmentOfficeRegionId);
            viewModel.AdministrativeDistricts = (await _cachedLookupService.AdministrativeDistrictsGetAllAsync()).ToSelectList(viewModel.AdministrativeDistrictId);
            viewModel.AdministrativeWards = (await _cachedLookupService.AdministrativeWardsGetAllAsync()).ToSelectList(viewModel.AdministrativeWardId);
            viewModel.ParliamentaryConstituencies = (await _cachedLookupService.ParliamentaryConstituenciesGetAllAsync()).ToSelectList(viewModel.ParliamentaryConstituencyId);
            viewModel.UrbanRuralLookup = (await _cachedLookupService.UrbanRuralGetAllAsync()).ToSelectList(viewModel.UrbanRuralId);
            viewModel.GSSLALookup = (await _cachedLookupService.GSSLAGetAllAsync()).ToSelectList(viewModel.GSSLAId);
            viewModel.CASWards = (await _cachedLookupService.CASWardsGetAllAsync()).ToSelectList(viewModel.CASWardId);
            viewModel.PruFulltimeProvisionOptions = (await _cachedLookupService.PruFulltimeProvisionsGetAllAsync()).ToSelectList(viewModel.PruFulltimeProvisionId);
            viewModel.PruEducatedByOthersOptions = (await _cachedLookupService.PruEducatedByOthersGetAllAsync()).ToSelectList(viewModel.PruEducatedByOthersId);
            viewModel.PRUEBDOptions = (await _cachedLookupService.PRUEBDsGetAllAsync()).ToSelectList(viewModel.PRUEBDId);
            viewModel.PRUSENOptions = (await _cachedLookupService.PRUSENsGetAllAsync()).ToSelectList(viewModel.PRUSENId);

            viewModel.Counties = (await _cachedLookupService.CountiesGetAllAsync()).ToSelectList(viewModel.Address_CountyId);
            viewModel.Countries = (await _cachedLookupService.NationalitiesGetAllAsync()).ToSelectList(viewModel.Address_CountryId);
            viewModel.OfstedRatings = (await _cachedLookupService.OfstedRatingsGetAllAsync()).ToSelectList(viewModel.OfstedRatingId);
            
            if (viewModel.MSOAId.HasValue) viewModel.MSOACode = (await _cachedLookupService.MSOAsGetAllAsync()).FirstOrDefault(x => x.Id == viewModel.MSOAId.Value)?.Code;
            if (viewModel.LSOAId.HasValue) viewModel.LSOACode = (await _cachedLookupService.LSOAsGetAllAsync()).FirstOrDefault(x => x.Id == viewModel.LSOAId.Value)?.Code;

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
            var parentGroup = parentGroups.FirstOrDefault(g => g.GroupTypeId == (int)eLookupGroupType.SingleacademyTrust);
            if (parentGroup != null)
            {
                return parentGroup;
            }

            parentGroup = parentGroups.FirstOrDefault(g => g.GroupTypeId == (int)eLookupGroupType.MultiacademyTrust);
            if (parentGroup != null)
            {
                return parentGroup;
            }

            parentGroup = parentGroups.FirstOrDefault(g => g.GroupTypeId == (int)eLookupGroupType.Trust);
            return parentGroup ?? parentGroups.FirstOrDefault();    
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
            vm.CASWardName = await c.GetNameAsync(() => vm.Establishment.CASWardId);
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
            vm.AltAddressCountyName = await c.GetNameAsync("CountyId", vm.Establishment.AltCountyId);
            vm.IEBTProprietorsAddressCountyName = await c.GetNameAsync("CountyId", vm.Establishment.IEBTModel.ProprietorsCountyId);
            vm.IEBTChairOfProprietorsBodyAddressCountyName = await c.GetNameAsync("CountyId", vm.Establishment.IEBTModel.ChairOfProprietorsBodyCountyId);

            if (vm.Establishment.LocalAuthorityId.HasValue)
            {
                var las = await c.LocalAuthorityGetAllAsync();
                var la = las.FirstOrDefault(x => x.Id == vm.Establishment.LocalAuthorityId);
                vm.LocalAuthorityName = la?.Name;
                vm.LocalAuthorityCode = la?.Code;
            }
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
                    return RedirectToAction(nameof(Details), new {id = response.Response});
                }
                else if (response.Errors.Any(x => x.Code == "establishment.with.same.name.la.postcode.found"))
                {
                    model.CCDuplicate = true;
                    ModelState.AddModelError(nameof(model.Name), "Please enter a different establishment name");
                    ModelState.AddModelError(nameof(model.LocalAuthorityId), "Please enter a different local authority");
                    ModelState.AddModelError("Address.PostCode", "Please enter a different postcode");
                }
                else response.ApplyToModelState(ControllerContext);    
            }

            await PopulateCCSelectLists(model);
            return View(model);
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

        private async Task PopulateSelectLists(CreateEstablishmentViewModel viewModel)
        {
            viewModel.LocalAuthorities = (await _cachedLookupService.LocalAuthorityGetAllAsync()).ToSelectList(viewModel.LocalAuthorityId);
            viewModel.EstablishmentTypes = (await _cachedLookupService.EstablishmentTypesGetAllAsync()).Where(x=> viewModel.CreateEstablishmentPermission.Types.Cast<int>().Contains(x.Id)).ToSelectList(viewModel.EstablishmentTypeId);
            viewModel.EducationPhases = (await _cachedLookupService.EducationPhasesGetAllAsync()).ToSelectList(viewModel.EducationPhaseId);
        }

        private async Task PopulateCCSelectLists(CreateChildrensCentreViewModel viewModel)
        {
            viewModel.PhaseId = 3;
            viewModel.OperationalHoursOptions = (await _cachedLookupService.CCOperationalHoursGetAllAsync()).ToSelectList();
            viewModel.GovernanceOptions = (await _cachedLookupService.CCGovernanceGetAllAsync()).ToSelectList();
            viewModel.DisadvantagedAreaOptions = (await _cachedLookupService.CCDisadvantagedAreasGetAllAsync()).ToSelectList();
            viewModel.DirectProvisionOfEarlyYearsOptions = (await _cachedLookupService.DirectProvisionOfEarlyYearsGetAllAsync()).ToSelectList();
            viewModel.EstablishmentStatusOptions = (await _cachedLookupService.EstablishmentStatusesGetAllAsync()).ToSelectList();
            viewModel.Address.Counties = (await _cachedLookupService.CountiesGetAllAsync()).ToSelectList();
            viewModel.Phases = (await _cachedLookupService.CCPhaseTypesGetAllAsync()).ToSelectList();
            await PopulateSelectLists(viewModel);
        }
    }
}