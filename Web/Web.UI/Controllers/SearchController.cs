using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using Web.Identity;
using Web.Services.Search;

namespace Web.UI.Controllers
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

        public ActionResult Results(string searchTerm)
        {
            if (searchTerm == null)
            {
                return RedirectToAction("Index");
            }

            dynamic model = new ExpandoObject();
            model.SearchTerm = searchTerm;
            model.Results = null;
            model.Error = null;
            ISearchSchoolsStrategy selectedStrategy = null;

            try
            {
                foreach (var strategy in _schoolSearchStrategieses.OrderBy(s => s.Priority))
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

            model.SearchType = selectedStrategy?.Description;

            return View(model);
        }
    }
}