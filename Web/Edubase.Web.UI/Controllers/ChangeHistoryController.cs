using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Lookup;
using Edubase.Services.Texuna.ChangeHistory;
using Edubase.Services.Texuna.ChangeHistory.Models;
using Edubase.Web.Resources;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("ChangeHistory"), Route("{action=index}"), EdubaseAuthorize]
    public class ChangeHistoryController : Controller
    {
        readonly IChangeHistoryService _svc;
        readonly ICachedLookupService _lookupService;

        public ChangeHistoryController(IChangeHistoryService svc, ICachedLookupService lookupService)
        {
            _svc = svc;
            _lookupService = lookupService;
        }

        [HttpGet, Route(Name = "ChangeHistoryCriteria")]
        public async Task<ActionResult> Index()
        {
            var vm = new ChangeHistoryViewModel();
            await PopulateLookups(vm);
            
            vm.SelectedEstablishmentFields = vm.EstablishmentFields.Select(x => x.Key).ToArray();
            vm.SelectedEstablishmentTypeIds = vm.EstablishmentTypes.Select(x => x.Id).ToList();
            vm.SelectedGroupTypeIds = vm.GroupTypes.Select(x => x.Id).ToList();
            
            return View(vm);
        }

        [HttpGet, Route("Search", Name = "ChangeHistorySearch")]
        public async Task<ActionResult> SearchChangeHistory(ChangeHistoryViewModel vm)
        {
            await PopulateLookups(vm);

            if (!vm.ClearResults && ModelState.IsValid)
            {
                if (!vm.StartDownload)
                {
                    var payload = PopulatePayload(vm, new SearchChangeHistoryBrowsePayload(vm.StartIndex, vm.PageSize));
                    vm.Results = await _svc.SearchAsync(payload, User);
                }
                else
                {
                    var payload = PopulatePayload(vm, new SearchChangeHistoryDownloadPayload(vm.DownloadFormat));
                    var progress = await _svc.SearchWithDownloadGenerationAsync(payload, User);
                    return Redirect(string.Concat(Url.RouteUrl("ChangeHistoryDownload", new { id = progress.Id }), "?", Request.QueryString));
                }
            }

            return View("Index", vm);
        }

        [HttpGet, Route("Download/{id}", Name = "ChangeHistoryDownload")]
        public async Task<ActionResult> SearchChangeHistoryDownload(Guid id, ChangeHistoryViewModel vm)
        {
            var progress = await _svc.GetDownloadGenerationProgressAsync(id, User);
            if (progress.IsComplete) return View("ReadyToDownload", new Tuple<ProgressDto, ChangeHistoryViewModel>(progress, vm));
            else return View("PreparingPleaseWait", progress);
        }
        
        private T PopulatePayload<T>(ChangeHistoryViewModel vm, T payload) where T : SearchChangeHistoryPayload
        {
            payload.EstablishmentFieldIds = vm.IsEstablishmentSearch && vm.SelectedEstablishmentFields.Any()
                                ? vm.SelectedEstablishmentFields : null;

            payload.EstablishmentTypeIds = vm.IsEstablishmentSearch && vm.SelectedEstablishmentTypeIds.Any()
                ? vm.SelectedEstablishmentTypeIds.ToArray() : null;

            payload.GroupTypeIds = vm.IsGroupSearch && vm.SelectedGroupTypeIds.Any()
                ? vm.SelectedGroupTypeIds.ToArray() : null;

            payload.EntityName = vm.IsGroupSearch ? "groups" : "establishments";
            payload.ApproverUserGroupCode = vm.SelectedApproverId.Clean();
            payload.SuggesterUserGroupCode = vm.SelectedSuggesterId.Clean();

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

        private async Task PopulateLookups(ChangeHistoryViewModel vm)
        {
            vm.EstablishmentFields = await _svc.GetEstablishmentFieldsAsync(User);
            vm.EstablishmentTypes = (await _lookupService.EstablishmentTypesGetAllAsync()).Select(x => new LookupItemViewModel(x));
            vm.GroupTypes = (await _lookupService.GroupTypesGetAllAsync()).Select(x => new LookupItemViewModel(x));

            var userGroups = (await _svc.GetSuggesterGroupsAsync(User));
            vm.Suggesters = userGroups.ToSelectList(vm.SelectedSuggesterId);
            vm.Approvers = userGroups.ToSelectList(vm.SelectedApproverId);
        }


    }
}