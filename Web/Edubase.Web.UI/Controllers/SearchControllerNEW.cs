using System.Linq;
using System.Web.Mvc;
using Edubase.Common;
using System.Web.Routing;
using Edubase.Data.Entity;
using Edubase.Web.UI.Models;
using System.Data.Entity;
using Edubase.Data;

namespace Edubase.Web.UI.Controllers
{
    using Services.Establishments;
    using Services.Groups;
    using System.Threading.Tasks;
    using ViewModel = AdvancedSearchViewModel;
    using System;
    using Services.Establishments.Search;
    using Services.Domain;
    using System.Collections.Generic;
    using Services;

    public class SearchController : EduBaseController
    {
        private IEstablishmentReadService _establishmentReadService;
        private IGroupReadService _groupReadService;

        public SearchController(IEstablishmentReadService establishmentReadService, IGroupReadService groupReadService)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }

        public ActionResult Index() => View();

        [HttpGet]
        public async Task<ActionResult> Results(ViewModel model)
        {
            if (model.SearchType == ViewModel.eSearchType.Text) return await SearchByTextSearch(model);
            //else if (model.SearchType == ViewModel.eSearchType.Location) return await SearchByLocation(model);
            //else if (model.SearchType == ViewModel.eSearchType.LocalAuthority) return await SearchByLocalAuthority(model);
            //else if (model.SearchType == ViewModel.eSearchType.Trust) return await SearchTrusts(model);
            //else if (model.SearchType == ViewModel.eSearchType.Governor) return SearchGovernors(model.GovernorSearchModel);
            //else throw new Exception("Unrecognised action");
            throw new NotImplementedException();
        }

        private async Task<ActionResult> SearchByTextSearch(ViewModel model)
        {
            var retVal = await SearchByUrnAsync(model);
            if (retVal != null) return retVal;
            
            var payload = new EstablishmentSearchPayload(nameof(SearchEstablishmentDocument.Name), model.StartIndex, model.PageSize);
            var filters = payload.Filters;
            if (model.TextSearchType == ViewModel.eTextSearchType.UKPRN)
            {
                filters.UKPRN = model.TextSearchModel.Text.ToInteger();
            }
            else if(model.TextSearchType == ViewModel.eTextSearchType.LAESTAB)
            {
                var laestab = LAESTAB.TryParse(model.TextSearchModel.Text).Value;
                filters.LocalAuthorityIds = new int[] { laestab.LocalAuthorityId };
                filters.EstablishmentNumber = laestab.EstablishmentNumber;
            }
            else if (model.TextSearchType == ViewModel.eTextSearchType.EstablishmentName)
            {
                payload.Text = model.TextSearchModel.Text;
            }
            else model.Error = "The LAESTAB, UKPRN or URN was invalid.";

            AddFilters(model, filters);

            return await ProcessSearch(model, payload);
        }

        private async Task<ActionResult> ProcessSearch(ViewModel model, EstablishmentSearchPayload payload)
        {
            if (!model.HasError)
            {
                var results = await _establishmentReadService.SearchAsync(payload);
                if (payload.Skip == 0) model.Count = results.Count.GetValueOrDefault();
                model.Results = results.Items;
            }

            if (model.Count == 1) return RedirectToEstabDetail(model.Results.First().Urn.GetValueOrDefault());
            else
            {
                var svc = new CachedLookupService();
                model.EstablishmentTypes = (await svc.EstablishmentTypesGetAllAsync()).Select(x => new LookupItemViewModel(x));
                model.EstablishmentStatuses = (await svc.EstablishmentStatusesGetAllAsync()).Select(x => new LookupItemViewModel(x));
                model.EducationPhases = (await svc.EducationPhasesGetAllAsync()).Select(x => new LookupItemViewModel(x));
                model.ReligiousCharacters = (await svc.ReligiousCharactersGetAllAsync()).Select(x => new LookupItemViewModel(x));
                model.LocalAuthorties = (await svc.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
                return View("AdvancedSearchResults", model);
            }
        }

        private void AddFilters(ViewModel viewModel, EstablishmentSearchFilters filters)
        {
            filters.EducationPhaseIds = viewModel.SelectedEducationPhaseIds.ToArray();
            filters.StatusIds = viewModel.SelectedEstablishmentStatusIds.ToArray();
            filters.TypeIds = viewModel.SelectedEstablishmentTypeIds.ToArray();
            filters.LocalAuthorityIds = viewModel.SelectedLocalAuthorityIds.ToArray();
            filters.ReligiousCharacterIds = viewModel.SelectedReligiousCharacterIds.ToArray();
        }


        private async Task<ActionResult> SearchByUrnAsync(ViewModel model)
        {
            var urn = model.TextSearchModel.AutoSuggestValueAsInt ?? (model.TextSearchType == ViewModel.eTextSearchType.URN ? model.TextSearchModel.Text.ToInteger() : null);
            if (urn.HasValue)
            {
                if (await _establishmentReadService.ExistsAsync(urn.Value, User))
                    return RedirectToEstabDetail(urn.Value);
            }
            return null;
        }


        public async Task<ActionResult> SearchByMATAS(string searchTerm, int startIndex = 0, int pageSize = 50)
        {
            return null;
        }

        [HttpGet]
        public async Task<ActionResult> Suggest(string text) => Json(await _establishmentReadService.SuggestAsync(text));

        [HttpGet]
        public async Task<ActionResult> SuggestTrust(string text) => Json(await _groupReadService.SuggestAsync(text));


        private ActionResult RedirectToEstabDetail(int urn)
            => new RedirectToRouteResult(null, new RouteValueDictionary
            {
                { "action", "Details" },
                { "controller", "Establishment" },
                { "id", urn }
            });

    }
}