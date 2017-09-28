using System.Linq;
using Edubase.Common;
using Edubase.Services.Core;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
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

        public ChangeHistoryController(IChangeHistoryService svc, ICachedLookupService lookupService, IEstablishmentReadService establishmentReadService)
        {
            _svc = svc;
            _lookupService = lookupService;
            _establishmentReadService = establishmentReadService;
        }

        [HttpGet, Route(Name = "ChangeHistoryCriteria")]
        public async Task<ActionResult> Index(ChangeHistoryViewModel viewModel)
        {
            return View(viewModel);
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

                        if (viewModel.SearchType == eSearchType.Group)
                        {
                            return Redirect(Url.RouteUrl("ChangeHistoryGroups") + "?" + Request.QueryString);
                        }

                        throw new NotSupportedException($"The search type '{viewModel.SearchType}' is not recognised.");
                    }
                }
            }

            return await Index(viewModel);

            //if (!string.IsNullOrWhiteSpace(vm.SearchType) && !vm.ClearResults && ModelState.IsValid)
            //{
            //    if (!vm.StartDownload)
            //    {
            //        var payload = PopulatePayload(vm, new SearchChangeHistoryBrowsePayload(vm.StartIndex, vm.PageSize));
            //        vm.Results = await _svc.SearchAsync(payload, User);
            //    }
            //    else
            //    {
            //        var payload = PopulatePayload(vm, new SearchChangeHistoryDownloadPayload(vm.DownloadFormat));
            //        var progress = await _svc.SearchWithDownloadGenerationAsync(payload, User);
            //        return Redirect(string.Concat(Url.RouteUrl("ChangeHistoryDownload", new { id = progress.Id }), "?", Request.QueryString));
            //    }
            //}
            //else
            //{
            //    ModelState.AddModelError("SearchType", "Please select either 'All establishments' or 'All groups'");
            //}

            //return View("Index", vm);
        }

        [HttpGet, Route("Search/Establishments", Name = "ChangeHistoryEstablishments")]
        public async Task<ActionResult> SearchChangeHistoryEstab(ChangeHistoryViewModel viewModel)
        {
            if (viewModel.SearchType == eSearchType.Text)
            {
                if (viewModel.TextSearchType == ChangeHistoryViewModel.eTextSearchType.URN)
                {
                    var urn = Int32.Parse(viewModel.TextSearchModel.Text);
                    var establishmentName = (await _establishmentReadService.GetEstablishmentNameAsync(urn, User))?? "";
                    viewModel.EstablishmentName = establishmentName;
                    var establishmentChanges = await _establishmentReadService.GetChangeHistoryAsync(urn, viewModel.StartIndex, viewModel.PageSize, User);
                    viewModel.Changes = ConvertEstablishmentChanges(establishmentChanges, establishmentName);
                    viewModel.SingleEstablishment = true;
                }
                else
                {
                    var result = await TryGetEstablishmentUrn(viewModel);
                    if (result?.Urn != null)
                    {
                        viewModel.EstablishmentName = result.Name;
                        var establishmentChanges = await _establishmentReadService.GetChangeHistoryAsync(
                            result.Urn.Value, viewModel.StartIndex, viewModel.PageSize,
                            User);
                        viewModel.Changes = ConvertEstablishmentChanges(establishmentChanges, result.Name);
                        viewModel.SingleEstablishment = true;
                    }
                    else
                    {
                        //TODO: redirect to change history search page with error message
                    }
                }
            }
            else
            {
                var payload = PopulatePayload(viewModel, new SearchChangeHistoryBrowsePayload(viewModel.StartIndex, viewModel.PageSize));
                var changes = await _svc.SearchAsync(payload, User);
                viewModel.Changes = new PaginatedResult<ChangeHistorySearchItem>(viewModel.StartIndex, viewModel.PageSize, changes);
            }
            
            return View("Results", viewModel);
        }

        private PaginatedResult<ChangeHistorySearchItem> ConvertEstablishmentChanges(PaginatedResult<EstablishmentChangeDto> changes, string estabName)
        {
            return new PaginatedResult<ChangeHistorySearchItem>(
                changes.Skip,
                changes.Take,
                changes.Count,
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

        [HttpGet, Route("Search/Groups", Name = "ChangeHistoryGroups")]
        public async Task<ActionResult> SearchChangeHistoryGroup(ChangeHistoryViewModel viewModel)
        {
            return View("Results", viewModel);
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
            payload.EstablishmentFieldIds = vm.IsEstablishmentSearch && vm.SelectedEstablishmentFields.Any() ? vm.SelectedEstablishmentFields : null;
            payload.EstablishmentTypeIds = vm.IsEstablishmentSearch && vm.SelectedEstablishmentTypeIds.Any() ? vm.SelectedEstablishmentTypeIds.ToArray() : null;
            //payload.GroupTypeIds = vm.IsGroupSearch && vm.SelectedGroupTypeIds.Any() ? vm.SelectedGroupTypeIds.ToArray() : null;

            payload.EntityName = vm.IsEstablishmentSearch ? "establishments" : "groups";
            //payload.ApproverUserGroupCode = vm.SelectedApproverId.Clean();
            //payload.SuggesterUserGroupCode = vm.SelectedSuggesterId.Clean();

            //if (vm.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_APPLIED)
            //{
            //    payload.AppliedDateFrom = vm.DateFilterFrom.ToDateTime();
            //    payload.AppliedDateTo = vm.DateFilterTo.ToDateTime();
            //}
            //else if (vm.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_APPROVED)
            //{
            //    payload.ApprovedDateFrom = vm.DateFilterFrom.ToDateTime();
            //    payload.ApprovedDateTo = vm.DateFilterTo.ToDateTime();
            //}
            //else if (vm.DateFilterMode == ChangeHistoryViewModel.DATE_FILTER_MODE_EFFECTIVE)
            //{
            //    payload.EffectiveDateFrom = vm.DateFilterFrom.ToDateTime();
            //    payload.EffectiveDateTo = vm.DateFilterTo.ToDateTime();
            //}
            return payload;
        }

        private async Task PopulateLookups(ChangeHistoryViewModel vm)
        {
            //vm.EstablishmentFields = await _svc.GetEstablishmentFieldsAsync(User);
            //vm.EstablishmentTypes = (await _lookupService.EstablishmentTypesGetAllAsync()).Select(x => new LookupItemViewModel(x));
            //vm.GroupTypes = (await _lookupService.GroupTypesGetAllAsync()).Select(x => new LookupItemViewModel(x));

            //var userGroups = (await _svc.GetSuggesterGroupsAsync(User));
            //vm.Suggesters = userGroups.ToSelectList(vm.SelectedSuggesterId);
            //vm.Approvers = userGroups.ToSelectList(vm.SelectedApproverId);
        }

        private async Task<EstablishmentSearchResultModel> TryGetEstablishmentUrn(ChangeHistoryViewModel model)
        {
            //var retVal = new Returns<EstablishmentSearchPayload>();
            var payload = new EstablishmentSearchPayload(model.StartIndex, model.PageSize);
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

            return results.Count == 1 ? results.Items.First() : null;
        }
    }
}