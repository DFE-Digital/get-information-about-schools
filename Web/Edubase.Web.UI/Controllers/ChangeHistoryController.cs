using System.Collections.Generic;
using System.Linq;
using Edubase.Common;
using Edubase.Services.Core;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Downloads;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Downloads;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using Edubase.Web.UI.Models.Search;
using Newtonsoft.Json;

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
        private readonly IGroupDownloadService _groupDownloadService;

        public ChangeHistoryController(IChangeHistoryService svc, ICachedLookupService lookupService, IEstablishmentReadService establishmentReadService, IGroupReadService groupReadService, IGroupDownloadService groupDownloadService)
        {
            _svc = svc;
            _lookupService = lookupService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _groupDownloadService = groupDownloadService;
        }

        [HttpGet, Route(Name = "ChangeHistoryCriteria")]
        public ActionResult Index(ChangeHistoryViewModel viewModel)
        {
            return View("Index", viewModel);    
        }

        [HttpGet, Route("Search", Name = "ChangeHistorySearch")]
        public async Task<ActionResult> SearchChangeHistory(ChangeHistoryViewModel viewModel)
        {
            if (viewModel.SearchType == eSearchType.Text  && viewModel.NoResultsForName)
            {
                var errorMessage = "We could not find any establishments matching your search criteria";
                if (viewModel.TextSearchModel.Text == string.Empty)
                {
                    errorMessage = "Please enter an establishment name, URN, LAESTAB or UKPRN to start a search";
                }

                ModelState.AddModelError("TextSearchModel.Text", errorMessage);
            }

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
            if (viewModel.SearchType == null)
            {
                viewModel.SearchType = eSearchType.EstablishmentAll;
            }

            if (viewModel.IsEstablishmentSearch && viewModel.SearchType == eSearchType.Text && viewModel.TextSearchModel.Text.IsNullOrEmpty())
            {
                ModelState.AddModelError("TextSearchModel.Text", "Please enter an establishment name, URN, LAESTAB or UKPRN to start a search");
                return View("Index", viewModel);
            }

            viewModel = await ProcessEstablishmentSearch(viewModel);
            if (viewModel.NoResultsForName)
            {
                return View("Index", viewModel);
            }

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
            if (viewModel.SearchType == eSearchType.Group && viewModel.GroupSearchModel.Text.IsNullOrEmpty())
            {
                ModelState.AddModelError("GroupSearchModel.Text", "We could not find any establishment groups matching your search criteria");
                return View("Index", viewModel);
            }

            viewModel = await ProcessGroupSearch(viewModel);
            if (viewModel.GroupSearchError)
                return View("Index", viewModel);

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

        [HttpGet, Route("Search/Download", Name="DownloadChangeHistory")]
        public async Task<ActionResult> Download(ChangeHistoryViewModel viewModel)
        {
            if (!viewModel.DownloadFormat.HasValue)
            {
                if (viewModel.DisplayErrors)
                {
                    ModelState.AddModelError(nameof(viewModel.DownloadFormat), "A file format must be selected");
                }
                return View("Download", viewModel);
            }

            if (viewModel.IsEstablishmentSearch)
            {
                if (viewModel.SearchType == eSearchType.Text)
                {
                    var result = await TryGetEstablishment(viewModel);

                    var payload = new EstablishmentChangeHistoryDownloadFilters
                    {
                        ApprovedBy = viewModel.ApprovedBy,
                        FieldsUpdated = viewModel.SelectedEstablishmentFields.Any() ? viewModel.SelectedEstablishmentFields.ToArray() : null,
                        FileFormat = viewModel.DownloadFormat == eFileFormat.CSV ? DownloadType.csv : DownloadType.xlsx,
                        SuggestedBy = viewModel.SuggestedBy
                    };

                    if (viewModel.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_APPLIED)
                    {
                        payload.DateAppliedFrom = viewModel.DateFilterFrom?.ToDateTime();
                        payload.DateAppliedTo = viewModel.DateFilterTo?.ToDateTime();
                    }
                    else if (viewModel.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_APPROVED)
                    {
                        payload.DateApprovedFrom = viewModel.DateFilterFrom?.ToDateTime();
                        payload.DateApprovedTo = viewModel.DateFilterTo?.ToDateTime();
                    }
                    else if (viewModel.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_EFFECTIVE)
                    {
                        payload.DateEffectiveFrom = viewModel.DateFilterFrom?.ToDateTime();
                        payload.DateEffectiveTo = viewModel.DateFilterTo?.ToDateTime();
                    }

                    var progress = await _establishmentReadService.GetChangeHistoryDownloadAsync(result.Item1, payload, User);
                    return View("ReadyToDownload",
                        new Tuple<ProgressDto, ChangeHistoryViewModel>(
                            new ProgressDto
                            {
                                FileLocationUri = progress.Url,
                                FileSizeInBytes = progress.FileSizeInBytes
                            }, viewModel));
                }
                else
                {
                    var payload = PopulatePayload(viewModel,
                        new SearchChangeHistoryDownloadPayload(viewModel.DownloadFormat.Value));
                    var progress = await _svc.SearchWithDownloadGenerationAsync(payload, User);
                    return Redirect(string.Concat(Url.RouteUrl("ChangeHistoryDownload", new { id = progress.Id }),
                        "?", Request.QueryString));
                }
            }

            if (viewModel.SearchType == eSearchType.Group)
            {
                var groupInfo = await TryGetGoupUid(viewModel);
                var progress = await _groupDownloadService.DownloadGroupHistory(groupInfo.Item1,
                    viewModel.DownloadFormat == eFileFormat.CSV ? DownloadType.csv : DownloadType.xlsx,
                    viewModel.DateFilterFrom?.ToDateTime(), viewModel.DateFilterTo?.ToDateTime(),
                    viewModel.SuggestedBy, User);
                return View("ReadyToDownload",
                    new Tuple<ProgressDto, ChangeHistoryViewModel>(
                        new ProgressDto
                        {
                            FileLocationUri = progress.Url,
                            FileSizeInBytes = progress.FileSizeInBytes
                        }, viewModel));
            }

            var payloadAll = PopulatePayload(viewModel,
                new SearchChangeHistoryDownloadPayload(viewModel.DownloadFormat.Value));
            var progressAll = await _svc.SearchWithDownloadGenerationAsync(payloadAll, User);
            return Redirect(string.Concat(Url.RouteUrl("ChangeHistoryDownload", new {id = progressAll.Id}),
                "?", Request.QueryString));

        }

        private async Task<ChangeHistoryViewModel> ProcessEstablishmentSearch(ChangeHistoryViewModel viewModel)
        {
            if (viewModel.SearchType == eSearchType.Text)
            {
                var result = await TryGetEstablishment(viewModel);
                if (result != null)
                {
                    viewModel.EstablishmentName = result.Item2;
                    var establishmentChanges = await _establishmentReadService.GetChangeHistoryAsync(result.Item1, viewModel.Skip, viewModel.Take, viewModel.Sortby, GetEstablishmentChangeHistoryFilters(viewModel), User);
                    viewModel.Items = ConvertEstablishmentChanges(establishmentChanges, result.Item2);
                    viewModel.Count = establishmentChanges.Count;
                    viewModel.SingleEstablishment = true;
                }
                else
                {
                    viewModel.NoResultsForName = true;
                    ModelState.AddModelError("TextSearchModel.Text", "We could not find any establishments matching your search criteria");
                }
            }
            else
            {
                var payload = PopulatePayload(viewModel, new SearchChangeHistoryBrowsePayload(viewModel.Skip, viewModel.Take));
                var changes = await _svc.SearchAsync(payload, User);
                viewModel.Items = new List<ChangeHistorySearchItem>(changes.Items);
                viewModel.Count = changes.Count;

                if (viewModel.Count == 0)
                {
                    ModelState.AddModelError("searchby-all-ref", "We could not find any establishments matching your search criteria");
                    viewModel.NoResultsForName = true;
                }
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
                    var groupInfo = await TryGetGoupUid(viewModel);

                    if (groupInfo != null)
                    {
                        var changes = await _groupReadService.GetChangeHistoryAsync(groupInfo.Item1, viewModel.Skip, viewModel.Take, viewModel.Sortby, viewModel.DateFilterFrom?.ToDateTime(), viewModel.DateFilterTo?.ToDateTime(), viewModel.SuggestedBy, User);
                        viewModel.Items = ConvertGroupChanges(changes, groupInfo.Item2);
                        viewModel.Count = changes.Count;
                        viewModel.SingleGroup = true;
                        viewModel.GroupName = groupInfo.Item2;
                    }
                    else
                    {
                        viewModel.GroupSearchError = true;
                        ModelState.AddModelError("GroupSearchModel.Text", "We could not find any establishment groups matching your search criteria");
                    }

                    break;
                case eSearchType.GroupAll:
                    var payload = PopulatePayload(viewModel, new SearchChangeHistoryBrowsePayload(viewModel.Skip, viewModel.Take));
                    var allChanges = await _svc.SearchAsync(payload, User);
                    viewModel.Items = new List<ChangeHistorySearchItem>(allChanges.Items);
                    viewModel.Count = allChanges.Count;
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
                        DateChanged = i.ChangedDateUtc,
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
                    DateChanged = i.ChangedDateUtc,
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

        [HttpGet, Route("DownloadAjax/{id}", Name = "ChangeHistoryDownloadAjax")]
        public async Task<ActionResult> SearchChangeHistoryDownloadAjax(Guid id, ChangeHistoryViewModel vm)
        {
            var progress = await _svc.GetDownloadGenerationProgressAsync(id, User);
            if (vm.SingleEstablishment || vm.SingleGroup)
            {
                return Json(
                    JsonConvert.SerializeObject(new
                    {
                        status = progress.IsComplete, redirect = "/ChangeHistory/Search/Download/"
                    }), JsonRequestBehavior.AllowGet);
            }

            return Json(
                JsonConvert.SerializeObject(new
                {
                    status = progress.IsComplete, redirect = string.Concat("/ChangeHistory/Download/", id)
                }), JsonRequestBehavior.AllowGet);

        }

        private T PopulatePayload<T>(ChangeHistoryViewModel vm, T payload) where T : SearchChangeHistoryPayload
        {
            payload.EstablishmentFieldIds = vm.IsEstablishmentSearch && vm.SelectedEstablishmentFields.Any() ? vm.SelectedEstablishmentFields.ToArray() : null;
            payload.EstablishmentTypeIds = vm.IsEstablishmentSearch && vm.SelectedEstablishmentTypeIds.Any() ? vm.SelectedEstablishmentTypeIds.ToArray() : null;
            payload.GroupTypeIds = !vm.IsEstablishmentSearch && vm.SelectedGroupTypeIds.Any() ? vm.SelectedGroupTypeIds.ToArray() : null;

            payload.EntityName = vm.IsEstablishmentSearch ? "establishments" : "groups";
            payload.ApproverUserGroupCode = vm.ApprovedBy.Clean();
            payload.SuggesterUserGroupCode = vm.SuggestedBy.Clean();
            payload.SortBy = vm.Sortby;

            if (vm.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_APPLIED)
            {
                payload.AppliedDateFrom = vm.DateFilterFrom?.ToDateTime();
                payload.AppliedDateTo = vm.DateFilterTo?.ToDateTime();
            }
            else if (vm.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_APPROVED)
            {
                payload.ApprovedDateFrom = vm.DateFilterFrom?.ToDateTime();
                payload.ApprovedDateTo = vm.DateFilterTo?.ToDateTime();
            }
            else if (vm.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_EFFECTIVE)
            {
                payload.EffectiveDateFrom = vm.DateFilterFrom?.ToDateTime();
                payload.EffectiveDateTo = vm.DateFilterTo?.ToDateTime();
            }
            return payload;
        }

        private async Task PopulateLists(ChangeHistoryViewModel model)
        {
            model.SuggesterGroups = (await _svc.GetSuggesterGroupsAsync(User)).OrderBy(s => s.Name).Select(s => new SelectListItem { Text = s.Name, Value = s.Code });
            model.ApproverGroups = model.SuggesterGroups;
            model.EstablishmentTypes = (await _lookupService.EstablishmentTypesGetAllAsync()).OrderBy(e => e.Name).Select(e => new LookupItemViewModel(e));
            model.GroupTypes = (await _lookupService.GroupTypesGetAllAsync()).OrderBy(g => g.Name).Select(g => new LookupItemViewModel(g));
            model.EstablishmentFields = (await _svc.GetEstablishmentFieldsAsync(User)).OrderBy(e => e.Text).Select(s => new StringLookupItemViewModel(s.Key, s.Text));
        }

        private async Task<Tuple<int, string>> TryGetEstablishment(ChangeHistoryViewModel viewModel)
        {
            if (viewModel.TextSearchType == ChangeHistoryViewModel.eTextSearchType.URN)
            {
                var urn = Int32.Parse(viewModel.TextSearchModel.Text);
                var establishmentName = (await _establishmentReadService.GetEstablishmentNameAsync(urn, User)) ?? "";
                return new Tuple<int, string>(urn, establishmentName);
            }
            else
            {
                int? urn = null;
                string name = null;
                if (viewModel.TextSearchModel.AutoSuggestValueAsInt.HasValue)
                {
                    urn = viewModel.TextSearchModel.AutoSuggestValueAsInt;
                    name = (await _establishmentReadService.GetEstablishmentNameAsync(urn.Value, User)) ?? "";
                }
                else
                {
                    var payload = new EstablishmentSearchPayload(viewModel.Skip, viewModel.Take);
                    var filters = payload.Filters;

                    if (viewModel.SearchType == eSearchType.Text || viewModel.SearchType == eSearchType.EstablishmentAll)
                    {
                        if (viewModel.TextSearchType == ChangeHistoryViewModel.eTextSearchType.UKPRN)
                        {
                            filters.UKPRN = viewModel.TextSearchModel.Text;
                        }
                        else if (viewModel.TextSearchType == ChangeHistoryViewModel.eTextSearchType.LAESTAB)
                        {
                            var laestab = LAESTAB.TryParse(viewModel.TextSearchModel.Text).Value;
                            var localAuthorityId = (await _lookupService.LocalAuthorityGetAllAsync()).FirstOrDefault(x => x.Code == laestab.LocalAuthorityCode)?.Id;
                            if (localAuthorityId.HasValue) filters.LocalAuthorityIds = new int[] { localAuthorityId.Value };
                            filters.EstablishmentNumber = laestab.EstablishmentNumber;
                        }
                        else if (viewModel.TextSearchType == ChangeHistoryViewModel.eTextSearchType.EstablishmentName)
                        {
                            payload.Text = viewModel.TextSearchModel.Text;
                        }
                        else return null;
                    }

                    var results = await _establishmentReadService.SearchAsync(payload, User);

                    if (results != null && results.Items.Any())
                    {
                        urn = results.Items.First().Urn;
                        name = results.Items.First().Name;
                    }
                }

                if (urn.HasValue)
                {
                    return new Tuple<int, string>(urn.Value, name);
                }
            }

            return null;
        }

        private async Task<Tuple<int, string>> TryGetGoupUid(ChangeHistoryViewModel model)
        {
            int? groupUid= null;
            var groupName = "";

            if (model.GroupSearchModel.AutoSuggestValueAsInt.HasValue)
            {
                groupUid = model.GroupSearchModel.AutoSuggestValueAsInt.Value;
                var group = await _groupReadService.GetAsync(groupUid.Value, User);
                if (group.Success)
                {
                    groupName = group.ReturnValue.Name;
                }
            }

            if (groupUid == null || string.IsNullOrWhiteSpace(groupName))
            {
                ApiPagedResult<SearchGroupDocument> results = null;
                var searchText = model.GroupSearchModel.Text.Clean();
                if (searchText != null)
                    results =
                        await _groupReadService.SearchByIdsAsync(searchText, searchText.ToInteger(), searchText, searchText.ToInteger(), User);

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
                    groupUid = results.Items.First().GroupUId;
                    groupName = results.Items.First().Name;
                }
                else
                {
                    return null;
                }
            }

            return new Tuple<int, string>(groupUid.Value, groupName);
        }
    }
}
