using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy.Generators.Emitters;
using Edubase.Common;
using Edubase.Services.Core;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using Edubase.Web.UI.Areas.Establishments.Models.Search;
using Edubase.Web.UI.Models.Search;

namespace Edubase.Web.UI.Controllers
{
    using Filters;
    using Models;
    using Services.Domain;
    using Services.Lookup;
    using Services.Texuna.ChangeHistory;
    using Services.Texuna.ChangeHistory.Models;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [RoutePrefix("ChangeHistory"), Route("{action=index}"), EdubaseAuthorize]
    public class ChangeHistoryController : Controller
    {
        private readonly IChangeHistoryService _svc;
        private readonly ICachedLookupService _lookupService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;

        public ChangeHistoryController(IChangeHistoryService svc, ICachedLookupService lookupService, IEstablishmentReadService establishmentReadService, IGroupReadService groupReadService)
        {
            _svc = svc;
            _lookupService = lookupService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }

        [HttpGet, Route(Name = "ChangeHistoryCriteria")]
        public async Task<ActionResult> Index(ChangeHistoryViewModel viewModel)
        {
            return View("Index", viewModel);
        }

        [HttpGet, Route("Search", Name = "ChangeHistorySearch")]
        public async Task<ActionResult> SearchChangeHistory(ChangeHistoryViewModel viewModel)
        {
            if (!viewModel.NoResultsForName)
            {
                if (viewModel.SearchType.HasValue)
                {
                    if (ModelState.IsValid)
                    {
                        if (viewModel.SearchType.OneOfThese(eSearchType.Text, eSearchType.EstablishmentAll))
                        {
                            return Redirect(Url.RouteUrl("ChangeHistoryEstablishments") + "?" + Request.QueryString);
                        }

                        if (viewModel.SearchType.OneOfThese(eSearchType.Group, eSearchType.GroupAll))
                        {
                            return Redirect(Url.RouteUrl("ChangeHistoryGroups") + "?" + Request.QueryString);
                        }

                        throw new NotSupportedException($"The search type '{viewModel.SearchType}' is not recognised.");
                    }
                }
            }

            return await Index(viewModel);
        }

        [HttpGet, Route("Search/Establishments", Name = "ChangeHistoryEstablishments")]
        public async Task<ActionResult> SearchChangeHistoryEstab(ChangeHistoryViewModel viewModel)
        {
            viewModel = await ProcessEstablishmentSearch(viewModel);
            if (viewModel.NoResultsForName)
                return View("Index", viewModel);

            await PopulateLists(viewModel);
            return View("Results", viewModel);
        }

        [HttpGet, Route("Search/Establishments/results-js")]
        public async Task<PartialViewResult> EstablishmentResultsPartial(ChangeHistoryViewModel viewModel)
        {
            if (viewModel.SearchType == null)
            {
                viewModel.SearchType = eSearchType.EstablishmentAll;
            }
            viewModel = await ProcessEstablishmentSearch(viewModel);
            HttpContext.Response.Headers.Add("x-count", viewModel.Count.ToString());
            return PartialView("Partials/_EstablishmentResults", viewModel);
        }

        [HttpGet, Route("Search/Groups", Name = "ChangeHistoryGroups")]
        public async Task<ActionResult> SearchChangeHistoryGroups(ChangeHistoryViewModel viewModel)
        {
            viewModel = await ProcessGroupSearch(viewModel);
            if (viewModel.GroupSearchError)
                return Redirect(Url.RouteUrl("ChangeHistoryCriteria") + "?" + Request.QueryString);

            await PopulateLists(viewModel);
            return View("Results", viewModel);
        }

        [HttpGet, Route("Search/Groups/results-js")]
        public async Task<PartialViewResult> GroupResultsPartial(ChangeHistoryViewModel viewModel)
        {
            viewModel = await ProcessGroupSearch(viewModel);
            HttpContext.Response.Headers.Add("x-count", viewModel.Count.ToString());
            return PartialView("Partials/_GroupResults", viewModel);
        }

        private async Task<ChangeHistoryViewModel> ProcessEstablishmentSearch(ChangeHistoryViewModel viewModel)
        {
            if (viewModel.SearchType == eSearchType.Text)
            {
                if (viewModel.TextSearchType == ChangeHistoryViewModel.eTextSearchType.URN)
                {
                    var urn = Int32.Parse(viewModel.TextSearchModel.Text);
                    var establishmentName = (await _establishmentReadService.GetEstablishmentNameAsync(urn, User)) ?? "";
                    viewModel.EstablishmentName = establishmentName;
                    var establishmentChanges = await _establishmentReadService.GetChangeHistoryAsync(urn, viewModel.Skip, viewModel.Take, GetEstablishmentChangeHistoryFilters(viewModel), User);
                    viewModel.Items = ConvertEstablishmentChanges(establishmentChanges, establishmentName);
                    viewModel.Count = establishmentChanges.Count;
                    viewModel.SingleEstablishment = true;
                }
                else
                {
                    int? urn;
                    string name;
                    if (viewModel.TextSearchModel.AutoSuggestValueAsInt.HasValue)
                    {
                        urn = viewModel.TextSearchModel.AutoSuggestValueAsInt;
                        name = (await _establishmentReadService.GetEstablishmentNameAsync(urn.Value, User)) ?? "";
                    }
                    else
                    {
                        var result = await TryGetEstablishmentUrn(viewModel);
                        urn = result?.Urn;
                        name = result?.Name;
                    }

                    if (urn != null)
                    {
                        viewModel.EstablishmentName = name;
                        var establishmentChanges = await _establishmentReadService.GetChangeHistoryAsync(
                            urn.Value, viewModel.Skip, viewModel.Take, GetEstablishmentChangeHistoryFilters(viewModel),
                            User);
                        viewModel.Items = ConvertEstablishmentChanges(establishmentChanges, name);
                        viewModel.Count = establishmentChanges.Count;
                        viewModel.SingleEstablishment = true;
                    }
                    else
                    {
                        viewModel.NoResultsForName = true;
                        
                    }
                }
            }
            else
            {
                var payload = PopulatePayload(viewModel, new SearchChangeHistoryBrowsePayload(viewModel.Skip, viewModel.Take));
                var changes = await _svc.SearchAsync(payload, User);
                viewModel.Items = new List<ChangeHistorySearchItem>(changes.Items);
                viewModel.Count = changes.Count;
            }

            return viewModel;
        }

        private EstablishmentChangeHistoryFilters GetEstablishmentChangeHistoryFilters(ChangeHistoryViewModel viewModel)
        {
            var filters = new EstablishmentChangeHistoryFilters
            {
                ApprovedBy = viewModel.ApprovedBy,
                SuggestedBy = viewModel.SuggestedBy,
                FieldsUpdated = viewModel.SelectedEstablishmentFields.ToArray()
            };

            switch (viewModel.DateFilterMode)
            {
                case ChangeHistoryViewModel.DATE_FILTER_MODE_EFFECTIVE:
                    filters.DateEffectiveFrom = viewModel.DateFilterFrom?.ToDateTime();
                    filters.DateEffectiveTo = viewModel.DateFilterTo?.ToDateTime();
                    break;
                case ChangeHistoryViewModel.DATE_FILTER_MODE_APPLIED:
                    filters.DateAppliedFrom = viewModel.DateFilterFrom?.ToDateTime();
                    filters.DateAppliedTo = viewModel.DateFilterTo?.ToDateTime();
                    break;
                case ChangeHistoryViewModel.DATE_FILTER_MODE_APPROVED:
                    filters.DateApprovedFrom = viewModel.DateFilterFrom?.ToDateTime();
                    filters.DateApprovedTo = viewModel.DateFilterTo?.ToDateTime();
                    break;
            }

            return filters;
        }

        private async Task<ChangeHistoryViewModel> ProcessGroupSearch(ChangeHistoryViewModel viewModel)
        {
            switch (viewModel.SearchType)
            {
                case eSearchType.Group:
                    int? groupUid = null;
                    var groupName = "";
                    var found = false;

                    if (viewModel.GroupSearchModel.AutoSuggestValueAsInt.HasValue)
                    {
                        groupUid = viewModel.GroupSearchModel.AutoSuggestValueAsInt.Value;
                        var group = await _groupReadService.GetAsync(groupUid.Value, User);
                        if (group.Success)
                        {
                            groupName = group.ReturnValue.Name;
                            found = true;
                        }
                    }

                    if (groupUid == null || string.IsNullOrWhiteSpace(groupName))
                    {
                        var result = await TryGetGoupUid(viewModel);
                        if (result != null)
                        {
                            groupUid = result.GroupUId;
                            groupName = result.Name;
                        }
                    }

                    if (groupUid.HasValue)
                    {
                        var changes = await _groupReadService.GetChangeHistoryAsync(groupUid.Value, viewModel.Skip, viewModel.Take, User);
                        viewModel.Items = ConvertGroupChanges(changes, groupName);
                        viewModel.SingleGroup = true;
                        viewModel.GroupName = groupName;
                    }
                    else
                    {
                        //TODO: redirect to change history search page with error message
                    }

                    break;
                case eSearchType.GroupAll:
                    var payload = PopulatePayload(viewModel, new SearchChangeHistoryBrowsePayload(viewModel.Skip, viewModel.Take));
                    var allChanges = await _svc.SearchAsync(payload, User);
                    viewModel.Items = new List<ChangeHistorySearchItem>(allChanges.Items);
                    break;
                default:
                    viewModel.GroupSearchError = true;
                    break;
            }

            return viewModel;
        }

        private List<ChangeHistorySearchItem> ConvertEstablishmentChanges(PaginatedResult<EstablishmentChangeDto> changes, string estabName)
        {
            return new List<ChangeHistorySearchItem>(
                changes.Items.Select(i => new ChangeHistorySearchItem
                    {
                        EstablishmentName = estabName,
                        SuggesterName = i.OriginatorUserName,
                        ApproverName = i.ApproverUserName,
                        EffectiveDate = i.EffectiveDateUtc,
                        RequestedDate = i.RequestedDateUtc,
                        EstablishmentUrn = i.Urn,
                        FieldName = i.Name,
                        OldValue = i.OldValue,
                        NewValue = i.NewValue
                    }).ToList());
        }

        private List<ChangeHistorySearchItem> ConvertGroupChanges(PaginatedResult<GroupChangeDto> changes,
            string groupName)
        {
            return new List<ChangeHistorySearchItem>(
                changes.Items.Select(i => new ChangeHistorySearchItem
                {
                    GroupName = groupName,
                    ApproverName = i.ApproverUserName,
                    GroupUId = i.GroupUId,
                    RequestType = i.RequestType,
                    LinkType = i.LinkType,
                    LinkDateUtc = i.LinkDateUtc,
                    LinkUrn = i.LinkUrn,
                    LinkEstablishmentName = i.LinkEstablishmentName,
                    SuggesterName = i.OriginatorUserName,
                    EffectiveDate = i.EffectiveDateUtc,
                    RequestedDate = i.RequestedDateUtc,
                    FieldName = i.Name,
                    OldValue = i.OldValue,
                    NewValue = i.NewValue
                 }).ToList());
        }

        [HttpGet, Route("Download/{id}", Name = "ChangeHistoryDownload")]
        public async Task<ActionResult> SearchChangeHistoryDownload(Guid id, ChangeHistoryViewModel vm)
        {
            var progress = await _svc.GetDownloadGenerationProgressAsync(id, User);
            return progress.IsComplete
                ? View("ReadyToDownload", new Tuple<ProgressDto, ChangeHistoryViewModel>(progress, vm))
                : View("PreparingPleaseWait", progress);
        }
        
        private T PopulatePayload<T>(ChangeHistoryViewModel vm, T payload) where T : SearchChangeHistoryPayload
        {
            payload.EstablishmentFieldIds = vm.IsEstablishmentSearch && vm.SelectedEstablishmentFields.Any() ? vm.SelectedEstablishmentFields.ToArray() : null;
            payload.EstablishmentTypeIds = vm.IsEstablishmentSearch && vm.SelectedEstablishmentTypeIds.Any() ? vm.SelectedEstablishmentTypeIds.ToArray() : null;
            payload.GroupTypeIds = !vm.IsEstablishmentSearch && vm.SelectedGroupTypeIds.Any() ? vm.SelectedGroupTypeIds.ToArray() : null;

            payload.EntityName = vm.IsEstablishmentSearch ? "establishments" : "groups";
            payload.ApproverUserGroupCode = vm.ApprovedBy.Clean();
            payload.SuggesterUserGroupCode = vm.SuggestedBy.Clean();

            if (vm.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_APPLIED)
            {
                payload.AppliedDateFrom = vm.DateFilterFrom.ToDateTime();
                payload.AppliedDateTo = vm.DateFilterTo.ToDateTime();
            }
            else if (vm.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_APPROVED)
            {
                payload.ApprovedDateFrom = vm.DateFilterFrom.ToDateTime();
                payload.ApprovedDateTo = vm.DateFilterTo.ToDateTime();
            }
            else if (vm.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_EFFECTIVE)
            {
                payload.EffectiveDateFrom = vm.DateFilterFrom.ToDateTime();
                payload.EffectiveDateTo = vm.DateFilterTo.ToDateTime();
            }
            return payload;
        }

        private async Task PopulateLists(ChangeHistoryViewModel model)
        {
            model.SuggesterGroups = (await _svc.GetSuggesterGroupsAsync(User)).Select(s => new SelectListItem { Text = s.Name, Value = s.Code });
            model.ApproverGroups = model.SuggesterGroups;
            model.EstablishmentTypes = (await _lookupService.EstablishmentTypesGetAllAsync()).Select(e => new LookupItemViewModel(e));
            model.GroupTypes = (await _lookupService.GroupTypesGetAllAsync()).Select(g => new LookupItemViewModel(g));
            model.EstablishmentFields = (await _svc.GetEstablishmentFieldsAsync(User)).Select(s => new StringLookupItemViewModel(s.Key, s.Text));
        }

        private async Task<EstablishmentSearchResultModel> TryGetEstablishmentUrn(ChangeHistoryViewModel model)
        {
            var payload = new EstablishmentSearchPayload(model.Skip, model.Take);
            var filters = payload.Filters;

            if (model.SearchType == eSearchType.Text || model.SearchType == eSearchType.EstablishmentAll)
            {
                if (model.TextSearchType == ChangeHistoryViewModel.eTextSearchType.UKPRN)
                {
                    filters.UKPRN = model.TextSearchModel.Text;
                }
                else if (model.TextSearchType == ChangeHistoryViewModel.eTextSearchType.LAESTAB)
                {
                    var laestab = LAESTAB.TryParse(model.TextSearchModel.Text).Value;
                    var localAuthorityId = (await _lookupService.LocalAuthorityGetAllAsync()).FirstOrDefault(x => x.Code == laestab.LocalAuthorityCode)?.Id;
                    if (localAuthorityId.HasValue) filters.LocalAuthorityIds = new int[] { localAuthorityId.Value };
                    filters.EstablishmentNumber = laestab.EstablishmentNumber;
                }
                else if (model.TextSearchType == ChangeHistoryViewModel.eTextSearchType.EstablishmentName)
                {
                    payload.Text = model.TextSearchModel.Text;
                }
                else return null;
            }

            var results = await _establishmentReadService.SearchAsync(payload, User);

            return results.Count >= 1 ? results.Items.First() : null;
        }

        private async Task<SearchGroupDocument> TryGetGoupUid(ChangeHistoryViewModel model)
        {
            ApiPagedResult<SearchGroupDocument> results = null;
            var searchText = model.GroupSearchModel.Text.Clean();
            if (searchText != null) results = await _groupReadService.SearchByIdsAsync(searchText, searchText.ToInteger(), searchText, User);

            if (results == null || results.Count == 0)
            {
                var payload = new GroupSearchPayload(model.Skip, model.Take)
                {
                    Text = model.GroupSearchModel.Text.Clean()
                };

                results = await _groupReadService.SearchAsync(payload, User);
            }

            if (results != null && results.Count > 0)
            {
                return results.Items.First();
            }

            return null;
        }
    }
}