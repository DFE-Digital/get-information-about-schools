using Edubase.Common;
using Edubase.Web.UI.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System;

namespace Edubase.Web.UI.Controllers
{
    using Common.Spatial;
    using Models.Search;
    using Services;
    using Services.Domain;
    using Services.Establishments;
    using Services.Establishments.Search;
    using Services.Groups;
    using Services.Groups.Models;
    using Services.Groups.Search;
    using StackExchange.Profiling;
    using System;
    using System.Threading.Tasks;
    using ViewModel = AdvancedSearchViewModel;

    public partial class SearchController : EduBaseController
    {
        

        private Returns<EstablishmentSearchPayload> GetEstablishmentSearchPayload(ViewModel model)
        {
            var retVal = new Returns<EstablishmentSearchPayload>();
            var payload = new EstablishmentSearchPayload(nameof(SearchEstablishmentDocument.Name), model.StartIndex, model.PageSize);
            var filters = payload.Filters;

            if (model.SearchType == ViewModel.eSearchType.Text)
            {
                if (model.TextSearchType == ViewModel.eTextSearchType.UKPRN)
                {
                    filters.UKPRN = model.TextSearchModel.Text.ToInteger();
                }
                else if (model.TextSearchType == ViewModel.eTextSearchType.LAESTAB)
                {
                    var laestab = LAESTAB.TryParse(model.TextSearchModel.Text).Value;
                    filters.LocalAuthorityIds = new int[] { laestab.LocalAuthorityId };
                    filters.EstablishmentNumber = laestab.EstablishmentNumber;
                }
                else if (model.TextSearchType == ViewModel.eTextSearchType.EstablishmentName)
                {
                    payload.Text = model.TextSearchModel.Text;
                }
                else retVal.ErrorMessage = "The LAESTAB, UKPRN or URN was invalid.";
            }
            else if (model.SearchType == ViewModel.eSearchType.Location)
            {
                var coord = LatLon.Parse(model.LocationSearchModel.AutoSuggestValue);
                if (coord != null)
                {
                    payload.GeoSearchLocation = coord;
                    payload.GeoSearchMaxRadiusInKilometres = 10;
                    payload.GeoSearchOrderByDistance = true;
                }
                else retVal.ErrorMessage = "The co-ordinate could not be parsed.";
            }

            filters.EducationPhaseIds = model.SelectedEducationPhaseIds.ToArray();
            filters.StatusIds = model.SelectedEstablishmentStatusIds.ToArray();
            filters.TypeIds = model.SelectedEstablishmentTypeIds.ToArray();
            filters.LocalAuthorityIds = model.SelectedLocalAuthorityIds.ToArray();
            filters.ReligiousCharacterIds = model.SelectedReligiousCharacterIds.ToArray();

            return retVal.Set(payload);
        }

        private async Task<ActionResult> ProcessEstablishmentsSearch(ViewModel model, EstablishmentSearchPayload payload)
        {
            if (!model.HasError)
            {
                using (MiniProfiler.Current.Step("Invoking AZS search"))
                {
                    var results = await _establishmentReadService.SearchAsync(payload, User);
                    if (payload.Skip == 0) model.Count = results.Count.GetValueOrDefault();
                    model.Results = results.Items;
                }
            }

            if (model.Count == 1) return RedirectToEstabDetail(model.Results.First().Urn.GetValueOrDefault());
            else
            {
                var permittedStatusIds = _establishmentReadService.GetPermittedStatusIds(User);

                using (MiniProfiler.Current.Step("Populate lookups from CachedLookupService"))
                {
                    var svc = new CachedLookupService();
                    model.EstablishmentTypes = (await svc.EstablishmentTypesGetAllAsync()).Select(x => new LookupItemViewModel(x));
                    model.EstablishmentStatuses = (await svc.EstablishmentStatusesGetAllAsync()).Where(x => permittedStatusIds == null || permittedStatusIds.Contains(x.Id)).Select(x => new LookupItemViewModel(x));
                    model.EducationPhases = (await svc.EducationPhasesGetAllAsync()).Select(x => new LookupItemViewModel(x));
                    model.ReligiousCharacters = (await svc.ReligiousCharactersGetAllAsync()).Select(x => new LookupItemViewModel(x));
                    model.LocalAuthorties = (await svc.LocalAuthorityGetAllAsync()).OrderBy(x => x.Name).Select(x => new LookupItemViewModel(x));
                }
                
                return View("AdvancedSearchResults", model);
            }
        }
        
        private async Task<ActionResult> SearchByUrnAsync(ViewModel model)
        {
            var urn = model.TextSearchModel.AutoSuggestValueAsInt 
                ?? (model.TextSearchType == ViewModel.eTextSearchType.URN 
                ? model.TextSearchModel.Text.ToInteger() : null);

            if (urn.HasValue)
            {
                if ((await _establishmentReadService.CanAccess(urn.Value, User)).ReturnValue)
                    return RedirectToEstabDetail(urn.Value);
            }

            return null;
        }
        
        private ActionResult RedirectToEstabDetail(int urn)
            => new RedirectToRouteResult(null, new RouteValueDictionary
            {
                { "action", "Details" },
                { "controller", "Establishment" },
                { "id", urn }
            });

        
        

    }
}