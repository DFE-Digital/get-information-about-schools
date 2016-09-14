using Edubase.Data.Repositories;
using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Edubase.Common;

namespace Edubase.Web.UI.Controllers
{
    public class MATController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Search(string term)
        {
            var viewModel = new MATSearchResultsViewModel(term);
            var repo = new MATRepository();
            var results = repo.Search(term);

            if (results.Count == 0 && term.IsInteger())
            {
                viewModel.Error = "The companies house number does not match any MAT/Academy Sponsor; please check the number entered or try searching using MAT/Academy Sponsor name";
            }
            else if(results.Count == 0)
            {
                viewModel.Error = "Sorry, we could not find any matching records. Please check your spelling and try again.";
            }

            viewModel.Results = results;

            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            return Content("", "text/plain"); //View(model);
        }
    }
}