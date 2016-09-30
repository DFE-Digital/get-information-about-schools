using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using Web.Identity;
using Web.Services.Exceptions;
using Web.Services.Search;
using Edubase.Common;
using System.Linq;
using System.Web.Routing;
using Edubase.Data.Entity;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers
{
    [Authorize(Roles = IdentityConstants.AccessAllSchoolsRoleName)]
    public class SearchController : Controller
    {
        private readonly IEnumerable<ISearchSchoolsStrategy> _schoolSearchStrategieses;

        public SearchController(IEnumerable<ISearchSchoolsStrategy> schoolSearchStrategieses)
        {
            _schoolSearchStrategieses = schoolSearchStrategieses;
        }

        // GET: Search
        public ActionResult Index()
        {
            return View();
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
                        viewModel.Results = dc.Establishments.Where(x => x.Urn == id).ToList();
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
                        var query = dc.Establishments.Where(x => x.LocalAuthorityId == localAuthorityId && x.EstablishmentNumber == estabNo);
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
                    var query = dc.Establishments.Where(x => x.Name.Contains(searchTerm));
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
                    var query = dc.Establishments.Where(x => x.LocalAuthorityId == la.Id);
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
                var query = dc.Companies.Where(x => x.Name.Contains(searchTerm));
                viewModel.Count = query.Count();
                viewModel.Results = query.OrderBy(x => x.Name).Skip(startIndex).Take(pageSize).ToList();

                foreach (var result in viewModel.Results)
                {
                    result.EstablishmentCount = dc.Database.SqlQuery<int>("SELECT COUNT(1) FROM Establishment2Company WHERE Company_GroupUID = " + result.GroupUID).Single();
                }

            }
            viewModel.SearchType = "MAT/AS";

            //if (viewModel.Count == 1)
            //{
            //    return new RedirectToRouteResult(null, new RouteValueDictionary
            //    {
            //        { "action", "Details" },
            //        { "controller", "Schools" },
            //        { "id", viewModel.Results.Single().Urn }
            //    });
            //}

            viewModel.CalculatePageStats(pageSize);

            return View("MATASResults", viewModel);
        }
    }
}