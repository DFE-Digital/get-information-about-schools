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
    using Services;
    using System;
    using ViewModel = AdvancedSearchViewModel;
    using Areas.Governors.Models;
    using Common.Spatial;
    using System.Threading.Tasks;
    using Data.Entity.Lookups;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class SearchController : EduBaseController
    {
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult Advanced() => View(new ViewModel());

        [HttpGet]
        public async Task<ActionResult> Results(ViewModel model)
        {
            if (model.SearchType == ViewModel.eSearchType.Text) return await SearchByTextSearch(model);
            else if (model.SearchType == ViewModel.eSearchType.Location) return await SearchByLocation(model);
            else if (model.SearchType == ViewModel.eSearchType.LocalAuthority) return await SearchByLocalAuthority(model);
            else if (model.SearchType == ViewModel.eSearchType.Trust) return SearchTrusts(model);
            else if (model.SearchType == ViewModel.eSearchType.Governor) return SearchGovernors(model.GovernorSearchModel);
            else throw new Exception("Unrecognised action");
        }

        private async Task<ActionResult> SearchByTextSearch(ViewModel model)
        {
            var query = CreateTextSearchQuery(model, model.TextSearchModel);
            return await ProcessResult(model, query);
        }

        private IQueryable<Establishment> CreateTextSearchQuery(ViewModel model, ViewModel.Payload payload)
        {
            var query = GetEstablishmentsQuery();
            if (payload.AutoSuggestValue.IsInteger())
            {
                var id = payload.AutoSuggestValue.ToInteger();
                query = query.Where(x => x.Urn == id);
            }
            else if (payload.Text.IsInteger())
            {
                var id = payload.Text.ToInteger();
                if (model.TextSearchType == ViewModel.eTextSearchType.LAESTAB)
                {
                    var localAuthorityId = int.Parse(payload.Text.Substring(0, 3));
                    var estabNo = int.Parse(payload.Text.Substring(3, 4));
                    query = query.Where(x => x.LocalAuthorityId == localAuthorityId && x.EstablishmentNumber == estabNo);
                }
                else if (model.TextSearchType == ViewModel.eTextSearchType.UKPRN) query = query.Where(x => x.UKPRN == id);
                else if (model.TextSearchType == ViewModel.eTextSearchType.URN) query = query.Where(x => x.Urn == id);
                else model.Error = "The LAESTAB, UKPRN or URN was invalid.";
            }
            else if (model.TextSearchType == ViewModel.eTextSearchType.EstablishmentName) query = query.Where(x => x.Name.Contains(model.TextSearchModel.Text));
            else if (model.TextSearchType == ViewModel.eTextSearchType.Unknown) model.Error = "Search type was invalid";

            query = AddFilters(model, query);

            query = query.OrderBy(x => x.Name);

            return query;
        }

        private ActionResult SearchGovernors(SearchModel governorSearchModel)
        {
            return Redirect("/Governors/Search?Forename=" + governorSearchModel.Forename + "&Surname=" + governorSearchModel.Surname + "&RoleId=" + governorSearchModel.RoleId + "&IncludeHistoric=" + governorSearchModel.IncludeHistoric);
        }

        private async Task<ActionResult> SearchByLocalAuthority(ViewModel model)
        {
            var query = GetEstablishmentsQuery();
            var payload = model.LocalAuthoritySearchModel;
            if (payload.AutoSuggestValue.IsInteger()) query = query.Where(x => x.LocalAuthorityId == payload.AutoSuggestValueAsInt).OrderBy(x => x.Name);
            else
            {
                var la = new CachedLookupService().LocalAuthorityGetAll().FirstOrDefault(x => x.Name == payload.Text);
                if (la != null) query = query.Where(x => x.LocalAuthorityId == la.Id);
                else model.Error = "The local authority name was not found";
            }
            return await ProcessResult(model, query);
        }

        private async Task<ActionResult> ProcessResult(ViewModel model, IQueryable<Establishment> query)
        {
            if (!model.HasError)
            {
                query = AddFilters(model, query);
                model.Count = query.Count();
                model.Results = query.Skip(model.StartIndex).Take(model.PageSize).ToList();
            }

            if (model.Count == 1)
            {
                return new RedirectToRouteResult(null, new RouteValueDictionary
                {
                    { "action", "Details" },
                    { "controller", "Schools" },
                    { "id", model.Results.Single().Urn }
                });
            }
            else
            {
                var svc = new CachedLookupService();
                model.EstablishmentTypes = (await svc.EstablishmentTypesGetAllAsync()).Select(x => new LookupItemViewModel(x));
                model.EstablishmentStatuses = (await svc.EstablishmentStatusesGetAllAsync()).Select(x => new LookupItemViewModel(x));
                model.EducationPhases = (await svc.EducationPhasesGetAllAsync()).Select(x => new LookupItemViewModel(x));
                model.ReligiousCharacters = (await svc.ReligiousCharactersGetAllAsync()).Select(x => new LookupItemViewModel(x));
                model.LocalAuthorties = (await svc.LocalAuthorityGetAllAsync()).Select(x => new LookupItemViewModel(x));
                return View("AdvancedSearchResults", model);
            }
        }


        private async Task<ActionResult> SearchByLocation(ViewModel model)
        {
            var query = GetEstablishmentsQuery();
            var coord = LatLon.Parse(model.LocationSearchModel.AutoSuggestValue).ToDBGeography();
            if (coord != null)
            {
                query = query.Where(x => x.Location.Distance(coord) < 30000).OrderBy(x => x.Location.Distance(coord));
            }
            else model.Error = "The co-ordinate could not be parsed.";
            return await ProcessResult(model, query);
        }

        private ActionResult SearchTrusts(ViewModel model)
        {
            if (model.TrustSearchModel.AutoSuggestValueAsInt.HasValue)
            {
                return new RedirectToRouteResult(null, new RouteValueDictionary
                {
                    { "action", "Details" },
                    { "controller", "Trust" },
                    { "id", model.TrustSearchModel.AutoSuggestValueAsInt }
                });
            }
            else
            {
                return SearchByMATAS(model.TrustSearchModel.Text, model.StartIndex);
            }
        }

        public ActionResult Search(string searchTerm, int startIndex = 0, int pageSize = 50)
        {
            if (searchTerm.Clean() == null) return RedirectToAction("Index");

            var viewModel = new SearchResultsModel(searchTerm);
            viewModel.StartIndex = startIndex;

            if (searchTerm.IsInteger())
            {
                var id = searchTerm.ToInteger().Value;
                if (searchTerm.Length == 6)
                {
                    using (var dc = new ApplicationDbContext())
                    {
                        viewModel.Results = dc.Establishments.Include(x => x.Status)
                            .Where(x => x.Urn == id).ToList();
                        viewModel.Count = viewModel.Results.Count;
                    }
                    viewModel.SearchType = SearchResultsModel.eSearchType.URN;
                }
                else if (searchTerm.Length == 7)
                {
                    var localAuthorityId = int.Parse(searchTerm.Substring(0, 3));
                    var estabNo = int.Parse(searchTerm.Substring(3, 4));
                    using (var dc = new ApplicationDbContext())
                    {
                        var query = dc.Establishments.Include(x => x.Status)
                            .Where(x => x.LocalAuthorityId == localAuthorityId && x.EstablishmentNumber == estabNo);
                        viewModel.Count = query.Count();
                        viewModel.Results = query.OrderBy(x => x.Name).Skip(startIndex).Take(pageSize).ToList();
                    }
                    viewModel.SearchType = SearchResultsModel.eSearchType.LAESTAB;
                }
                else viewModel.Error = "The LAESTAB or URN was invalid.";
            }
            else
            {
                using (var dc = new ApplicationDbContext())
                {
                    var query = dc.Establishments.Include(x => x.Status)
                        .Where(x => x.Name.Contains(searchTerm));
                    viewModel.Count = query.Count();
                    viewModel.Results = query.OrderBy(x => x.Name).Skip(startIndex).Take(pageSize).ToList();
                }
                viewModel.SearchType = SearchResultsModel.eSearchType.SchoolName;
            }

            if (viewModel.Count == 1)
            {
                return new RedirectToRouteResult(null, new RouteValueDictionary
                                {
                                    { "action", "Details" },
                                    { "controller", "Schools" },
                                    { "id", viewModel.Results.Single().Urn }
                                });
            }

            viewModel.CalculatePageStats(pageSize);
            
            return View("Results", viewModel);
        }

        public ActionResult SearchByLaName(string searchTerm, int startIndex = 0, int pageSize = 50)
        {
            if (searchTerm.Clean() == null) return RedirectToAction("Index");

            var viewModel = new SearchResultsModel(searchTerm);
            viewModel.StartIndex = startIndex;
            
            using (var dc = new ApplicationDbContext())
            {
                var la = dc.LocalAuthorities.Where(x => x.Name.Equals(searchTerm)).FirstOrDefault();
                if (la != null)
                {
                    var query = dc.Establishments.Include(x => x.Status)
                        .Where(x => x.LocalAuthorityId == la.Id);
                    viewModel.Count = query.Count();
                    viewModel.Results = query.OrderBy(x => x.Name).Skip(startIndex).Take(pageSize).ToList();
                }
                else
                {
                    viewModel.Error = "The local authority name was not found";
                }
            }
            viewModel.SearchType = SearchResultsModel.eSearchType.LA;
        
            if (viewModel.Count == 1)
            {
                return new RedirectToRouteResult(null, new RouteValueDictionary
                                {
                                    { "action", "Details" },
                                    { "controller", "Schools" },
                                    { "id", viewModel.Results.Single().Urn }
                                });
            }

            viewModel.CalculatePageStats(pageSize);

            return View("Results", viewModel);
        }

        public ActionResult SearchByMATAS(string searchTerm, int startIndex = 0, int pageSize = 50)
        {
            if (searchTerm.Clean() == null) return RedirectToAction("Index");

            var viewModel = new MATASSearchResultsModel(searchTerm);
            viewModel.StartIndex = startIndex;

            using (var dc = new ApplicationDbContext())
            {
                IQueryable<Company> query = null;
                if (searchTerm.IsInteger()) query = dc.Companies.Where(x => x.CompaniesHouseNumber == searchTerm);
                else query = dc.Companies.Where(x => x.Name.Contains(searchTerm));

                viewModel.Count = query.Count();
                viewModel.Results = query.OrderBy(x => x.Name).Skip(startIndex).Take(pageSize).ToList();

                foreach (var result in viewModel.Results)
                {
                    result.EstablishmentCount = dc.Database
                        .SqlQuery<int>("SELECT COUNT(1) FROM Establishment2Company WHERE CompanyGroupUID = " + result.GroupUID).Single();
                }

            }
            viewModel.SearchType = "MAT/AS";

            if (viewModel.Count == 1)
            {
                return new RedirectToRouteResult(null, new RouteValueDictionary
                {
                    { "action", "Details" },
                    { "controller", "Trust" },
                    { "id", viewModel.Results.Single().GroupUID }
                });
            }

            viewModel.CalculatePageStats(pageSize);

            return View("MATASResults", viewModel);
        }
        
        [HttpGet]
        public ActionResult Suggest(string text) => Json(new EstablishmentService().Autosuggest(text));

        [HttpGet]
        public ActionResult SuggestTrust(string text) => Json(DataContext.Companies.Where(x => x.Name.StartsWith(text))
            .OrderBy(x=>x.Name).Take(10).Select(x => new { Name = x.Name, Id = x.GroupUID }));

        private IQueryable<Establishment> GetEstablishmentsQuery() => DataContext.Establishments.Include(x => x.Status);


        private IQueryable<Establishment> AddFilters(ViewModel viewModel, IQueryable<Establishment> query)
        {
            var selectedEducationPhaseIds = viewModel.SelectedEducationPhaseIds.Select(p => p.ToInteger()).ToArray();
            var selectedEstablishmentStatusIds = viewModel.SelectedEstablishmentStatusIds.Select(p => p.ToInteger()).ToArray();
            var selectedEstablishmentTypeIds = viewModel.SelectedEstablishmentTypeIds.Select(p => p.ToInteger()).ToArray();
            var selectedLocalAuthorityIds = viewModel.SelectedLocalAuthorityIds.Select(p => p.ToInteger()).ToArray();
            var selectedReligiousCharacterIds = viewModel.SelectedReligiousCharacterIds.Select(p => p.ToInteger()).ToArray();
            
            if (selectedEducationPhaseIds.Any()) query = query.Where(x => selectedEducationPhaseIds.Contains(x.EducationPhaseId));
            if (selectedEstablishmentStatusIds.Any()) query = query.Where(x => selectedEstablishmentStatusIds.Contains(x.StatusId));
            if (selectedEstablishmentTypeIds.Any()) query = query.Where(x => selectedEstablishmentTypeIds.Contains(x.TypeId));
            if (selectedLocalAuthorityIds.Any()) query = query.Where(x => selectedLocalAuthorityIds.Contains(x.LocalAuthorityId));
            if (selectedReligiousCharacterIds.Any()) query = query.Where(x => selectedReligiousCharacterIds.Contains(x.ReligiousCharacterId));

            return query;
        }

    }
}