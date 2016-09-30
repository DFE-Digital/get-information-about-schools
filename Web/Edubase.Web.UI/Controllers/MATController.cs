using Edubase.Data.Repositories;
using Edubase.Web.UI.Models;
using System.Web.Mvc;
using Edubase.Services.Query.Interfaces;
using Edubase.Data.Entity;
using System.Data.Entity;
using System.Linq;

namespace Edubase.Web.UI.Controllers
{
    public class MATController : Controller
    {
        ISchoolQueryService _schoolQueryService;

        public MATController(ISchoolQueryService schoolQueryService)
        {
            _schoolQueryService = schoolQueryService;
        }

        public ActionResult Details(short id)
        {
            using (var dc = new ApplicationDbContext())
            {
                var mat = dc.Companies.FirstOrDefault(x => x.GroupUID == id);
                var estabs = dc.Establishment2CompanyLinks.Include(x => x.Establishment)
                    .Where(x => x.Company.GroupUID == id).ToList();
                return View(new MATDetailViewModel(estabs, mat));
            }
        }
    }
}