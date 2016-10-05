using Edubase.Data.Entity;
using Edubase.Web.UI.Models;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers
{
    public class TrustController : Controller
    {
        private const string VIEWNAME = "CreateEdit";

        [HttpGet]
        public ActionResult Create() => View(VIEWNAME, new CreateEditTrustModel());

        [HttpPost]
        public async Task<ActionResult> Create(CreateEditTrustModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var dc = new ApplicationDbContext())
                {
                    var company = new Company
                    {
                        Name = viewModel.Name,
                        GroupTypeId = viewModel.TypeId,
                        OpenDate = viewModel.OpenDate.ToDateTime()
                    };
                    dc.Companies.Add(company);
                    await dc.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = company.GroupUID });
                }
            }
            else return View(VIEWNAME);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var viewModel = new CreateEditTrustModel();
            using (var dc = new ApplicationDbContext())
            {
                var company = await dc.Companies.FirstOrDefaultAsync(x => x.GroupUID == id);
                viewModel.GroupUID = company.GroupUID;
                viewModel.Name = company.Name;
                viewModel.TypeId = company.GroupTypeId;
                viewModel.OpenDate = new DateTimeViewModel(company.OpenDate);
            }
            return View(VIEWNAME, viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CreateEditTrustModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var dc = new ApplicationDbContext())
                {
                    var company = dc.Companies.Single(x => x.GroupUID == viewModel.GroupUID.Value);
                    company.Name = viewModel.Name;
                    company.OpenDate = viewModel.OpenDate.ToDateTime();
                    company.GroupTypeId = viewModel.TypeId;
                    await dc.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = company.GroupUID });
                }
            }
            else return View(VIEWNAME);
        }


        public ActionResult Details(short id)
        {
            using (var dc = new ApplicationDbContext())
            {
                var mat = dc.Companies.Include(x => x.GroupType).FirstOrDefault(x => x.GroupUID == id);
                var estabs = dc.Establishment2CompanyLinks.Include(x => x.Establishment)
                    .Include(x => x.Establishment.EstablishmentType)
                    .Include(x => x.Establishment.HeadTitle)
                    .Where(x => x.Company.GroupUID == id).ToList();
                return View(new MATDetailViewModel(estabs, mat));
            }
        }

    }
}