using Edubase.Common;
using Edubase.Services.Lookup;
using Edubase.Services.Texuna.ChangeHistory;
using Edubase.Web.Resources;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Models;
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

            if (!vm.ClearResults)
            {
                vm.Results = await _svc.SearchAsync(new Services.Texuna.ChangeHistory.Models.SearchChangeHistoryBrowsePayload
                {
                    EstablishmentFieldIds = vm.IsEstablishmentSearch && vm.SelectedEstablishmentFields.Any() ? vm.SelectedEstablishmentFields : null,
                    EstablishmentTypeIds = vm.IsEstablishmentSearch && vm.SelectedEstablishmentTypeIds.Any() ? vm.SelectedEstablishmentTypeIds.ToArray() : null,
                    GroupTypeIds = vm.IsGroupSearch && vm.SelectedGroupTypeIds.Any() ? vm.SelectedGroupTypeIds.ToArray() : null,
                    Skip = 0,
                    Take = 10,
                    EntityName = vm.IsGroupSearch ? "groups" : "establishments",
                    ApproverUserGroupCode = vm.SelectedApproverId.Clean(),
                    SuggesterUserGroupCode = vm.SelectedSuggesterId.Clean()
                }, User);
            }

            return View("Index", vm);
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