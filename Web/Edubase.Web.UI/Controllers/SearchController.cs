using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using Web.Identity;
using Web.Services.Exceptions;
using Web.Services.Search;
using Edubase.Common;
using System.Linq;

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

        public ActionResult Search(string searchTerm)
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
                        var results = strategy.Search(searchTerm);
                        if (results != null)
                        {
                            model.Results = results;
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

        public ActionResult SearchByLaName(string laName)
        {
            if (laName == null) return RedirectToAction("Index");
            
            dynamic model = new ExpandoObject();
            model.SearchType = "Search by local authority name";
            model.SearchTerm = laName;
            model.Results = null;
            model.Error = null;

            try
            {
                model.Results = _schoolSearchStrategieses.First(x => x is SearchSchoolsByLaNameStrategy).Search(laName);
            }
            catch (LaNameNotFoundException)
            {
                model.Error = "The local authority name was not found";
            }
            
            return View("Results", model);
        }
    }
}