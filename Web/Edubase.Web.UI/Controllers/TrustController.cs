using Edubase.Data.Entity;
using Edubase.Web.UI.Models;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using Edubase.Common;
using System;
using Edubase.Services;
using Edubase.Services.Domain;
using System.Security.Claims;

namespace Edubase.Web.UI.Controllers
{
    public class TrustController : Controller
    {
        private const string VIEWNAME = "CreateEdit";
        
        [Authorize(Roles = "Admin,Academy")]
        public async Task<ActionResult> SearchCompaniesHouse(SearchCompaniesHouseModel viewModel)
        {
            if (!viewModel.SearchText.IsNullOrEmpty())
            {
                var svc = new TrustService();
                if (viewModel.SearchText.IsInteger()) viewModel.Results = await svc.SearchByCompaniesHouseNumber(viewModel.SearchText);
                else viewModel.Results = await svc.SearchByName(viewModel.SearchText, viewModel.Skip);
                viewModel.NotFound = !viewModel.Results.Items.Any();
            }
            return View(viewModel);
        }

        [HttpGet, Authorize(Roles = "Admin,Academy")]
        public async Task<ActionResult> Create(string id)
        {
            var companyProfile = await new TrustService().SearchByCompaniesHouseNumber(id);
            return View("Create", new CreateTrustModel(companyProfile.Items.First()));
        }
        
        [HttpPost, Authorize(Roles = "Admin,Academy")]
        public async Task<ActionResult> Create(CreateTrustModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var id = await new TrustService().CreateAsync(User as ClaimsPrincipal, viewModel.CompaniesHouseNumber, viewModel.TypeId.Value);
                return RedirectToAction("Details", new { id });
            }
            else return View(viewModel);
        }

        [HttpGet, Authorize(Roles = "Admin,Academy")]
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
                viewModel.CompaniesHouseNumber = company.CompaniesHouseNumber;

                viewModel.Establishments = (await dc.Establishment2CompanyLinks
                    .Include(x => x.Establishment)
                    .Include(x => x.Establishment.EstablishmentType)
                    .Include(x => x.Establishment.HeadTitle)
                    .Where(x => x.Company.GroupUID == company.GroupUID)
                    .Select(x => x.Establishment)
                    .ToArrayAsync())
                    .Select(x => new TrustEstabViewModel(x)).ToList();
            }
            return View(VIEWNAME, viewModel);
        }

        [HttpPost, Authorize(Roles = "Admin,Academy")]
        public async Task<ActionResult> Edit(CreateEditTrustModel viewModel)
        {
            if (viewModel.Action == "Search")
            {
                ModelState.Clear();
                var urn = viewModel.SearchURN.ToInteger();
                using (var dc = new ApplicationDbContext()) viewModel.EstablishmentName = dc.Establishments.Where(x => x.Urn == urn).Select(x => x.Name).FirstOrDefault();
                if (viewModel.EstablishmentName != null) viewModel.EstablishmentUrn = urn;
                else viewModel.EstablishmentNotFound = true;
            }
            else if (viewModel.Action == "Add")
            {
                ModelState.Clear();
                if (!viewModel.Establishments.Any(x => x.Urn == viewModel.EstablishmentUrn))
                {
                    using (var dc = new ApplicationDbContext())
                    {
                        var estab = new TrustEstabViewModel(await dc.Establishments
                            .Include(x => x.HeadTitle)
                            .Include(x => x.EstablishmentType)
                            .FirstOrDefaultAsync(x => x.Urn == viewModel.EstablishmentUrn));
                        viewModel.Establishments.Insert(0, estab);
                        viewModel.SearchURN = string.Empty;
                        viewModel.EstablishmentUrn = null;
                    }
                }
            }
            else if (viewModel.Action.Equals("Remove", StringComparison.OrdinalIgnoreCase) && viewModel.EstabUrnToRemove.HasValue)
            {
                ModelState.Clear();
                var index = viewModel.Establishments.FindIndex(x => x.Urn == viewModel.EstabUrnToRemove);
                if (index >= 0) viewModel.Establishments.RemoveAt(index);
            }
            else if (viewModel.Action.Equals("Save", StringComparison.OrdinalIgnoreCase))
            {
                if (ModelState.IsValid)
                {
                    using (var dc = new ApplicationDbContext())
                    {
                        var company = await dc.Companies.SingleAsync(x => x.GroupUID == viewModel.GroupUID.Value);
                        company.Name = viewModel.Name;
                        company.OpenDate = viewModel.OpenDate.ToDateTime();
                        company.GroupTypeId = viewModel.TypeId;
                        company.CompaniesHouseNumber = viewModel.CompaniesHouseNumber.Clean();
                        
                        var links = dc.Establishment2CompanyLinks.Where(x => x.CompanyGroupUID == viewModel.GroupUID).ToList();
                        var urnsInDb = links.Select(x => x.EstablishmentUrn).Cast<int?>().ToArray();
                        var urnsInModel = viewModel.Establishments.Select(x => x.Urn).Cast<int?>().ToArray();
                        
                        var urnsToAdd = from e in urnsInModel
                                    join l in urnsInDb on e equals l into l2
                                    from l3 in l2.DefaultIfEmpty()
                                    where l3 == null
                                    select e;

                        var urnsToRemove = from l in urnsInDb
                                       join e in urnsInModel on l equals e into e2
                                       from e3 in e2.DefaultIfEmpty()
                                       where e3 == null
                                       select l;

                        foreach (var urn in urnsToAdd.Cast<int>())
                        {
                            var link = new Establishment2Company
                            {
                                CompanyGroupUID = company.GroupUID,
                                EstablishmentUrn = urn,
                                JoinedDate = DateTime.UtcNow
                            };
                            dc.Establishment2CompanyLinks.Add(link);
                        }

                        foreach (var urn in urnsToRemove.Cast<int>())
                        {
                            var o = links.FirstOrDefault(x => x.EstablishmentUrn == urn);
                            if (o != null) dc.Establishment2CompanyLinks.Remove(o);
                        }
                        
                        await dc.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = company.GroupUID });
                    }
                }
            }
            else throw new Exception("Action unrecognised");

            return View(VIEWNAME, viewModel);
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
                return View(new MATDetailViewModel(estabs, mat, User.Identity.IsAuthenticated));
            }
        }

    }
}