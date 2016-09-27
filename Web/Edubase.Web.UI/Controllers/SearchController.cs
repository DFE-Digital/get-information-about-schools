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

        public ActionResult Search(string searchTerm, int startIndex = 0, int pageSize = 20)
        {
            if (searchTerm == null) return RedirectToAction("Index");

            dynamic model = new ExpandoObject();
            model.SearchTerm = searchTerm;
            model.Results = null;
            model.Error = null;
            ISearchSchoolsStrategy selectedStrategy = null;

            if (searchTerm.IsInteger() && searchTerm.Length != 6 && searchTerm.Length != 7)
            {
                model.Error = "The LAESTAB or URN was invalid.";
            }
            else
            {

                try
                {
                    foreach (var strategy in _schoolSearchStrategieses.Where(s => !(s is SearchSchoolsByLaNameStrategy)).OrderBy(s => s.Priority))
                    {
                        selectedStrategy = strategy;
                        var results = strategy.Search(searchTerm)?.ToList();
                        if (results != null)
                        {
                            model.Results = results;

                            if (results.Count == 1)
                            {
                                return new RedirectToRouteResult(null, new RouteValueDictionary
                                {
                                    { "action", "Details" },
                                    { "controller", "Schools" },
                                    { "id", results.Single().id }
                                });
                            }
                            else
                            {
                                startIndex = (startIndex > results.Count) ? 0 : startIndex;

                                model.Page = results.Skip(startIndex).Take(pageSize).ToList();
                                model.StartIndex = startIndex;
                                model.PageCount = (int) Math.Ceiling(model.Results.Count/(double) pageSize);
                                model.PageSize = pageSize;
                            }
                            break;
                        }
                    }
                }
                catch (SearchException ex)
                {
                    model.Error = ex.Message;
                }
            }
            
            model.SearchType = selectedStrategy?.Description;

            return View("Results", model);
        }

        public ActionResult SearchByLaName(string searchTerm, int startIndex = 0, int pageSize = 20)
        {
            if (searchTerm == null) return RedirectToAction("Index");
            
            dynamic model = new ExpandoObject();
            model.SearchType = "Search by local authority name";
            model.SearchTerm = searchTerm;
            model.Results = null;
            model.Error = null;
            model.PageCount = 0;

            try
            {
                var results = _schoolSearchStrategieses.First(x => x is SearchSchoolsByLaNameStrategy).Search(searchTerm)?.ToList();
                model.Results = results;

                startIndex = (startIndex > model.Results.Count) ? 0 : startIndex;

                model.Page = results.Skip(startIndex).Take(pageSize).ToList();
                model.StartIndex = startIndex;
                model.PageCount = (int)Math.Ceiling(model.Results.Count / (double)pageSize);
                model.PageSize = pageSize;
            }
            catch (LaNameNotFoundException)
            {
                model.Error = "The local authority name was not found";
            }
            
            return View("Results", model);
        }
    }
}