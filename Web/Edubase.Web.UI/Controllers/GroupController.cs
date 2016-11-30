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
using Edubase.Web.UI.Helpers;
using Edubase.Services.Enums;


namespace Edubase.Web.UI.Controllers
{
    public class GroupController : Controller
    {
        private const string VIEWNAME = "CreateEdit";
        
        [Authorize]
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

        [HttpGet, Authorize]
        public async Task<ActionResult> Create(string id)
        {
            var companyProfile = await new TrustService().SearchByCompaniesHouseNumber(id);
            return View("Create", new CreateGroupModel(companyProfile.Items.First()));
        }
        
        [HttpPost, Authorize]
        public async Task<ActionResult> Create(CreateGroupModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var id = await new TrustService().CreateAsync(User as ClaimsPrincipal, viewModel.CompaniesHouseNumber, viewModel.TypeId.Value);
                return RedirectToAction("Details", new { id });
            }
            else return View(viewModel);
        }

        [HttpGet, Authorize]
        public async Task<ActionResult> Edit(int id)
        {
            var viewModel = new CreateEditGroupModel();
            using (var dc = new ApplicationDbContext())
            {
                var company = await dc.Groups.FirstOrDefaultAsync(x => x.GroupUID == id);
                viewModel.GroupUID = company.GroupUID;
                viewModel.Name = company.Name;
                viewModel.TypeId = company.GroupTypeId;
                viewModel.OpenDate = new DateTimeViewModel(company.OpenDate);
                viewModel.CompaniesHouseNumber = company.CompaniesHouseNumber;

                viewModel.Establishments = (await dc.EstablishmentTrusts
                    .Include(x => x.Establishment)
                    .Include(x => x.Establishment.EstablishmentType)
                    .Include(x => x.Establishment.HeadTitle)
                    .Where(x => x.Trust.GroupUID == company.GroupUID)
                    .Select(x => x.Establishment)
                    .ToArrayAsync())
                    .Select(x => new GroupEstabViewModel(x)).ToList();
            }
            return View(VIEWNAME, viewModel);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> Edit(CreateEditGroupModel viewModel)
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
                        var estab = new GroupEstabViewModel(await dc.Establishments
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
                        var company = await dc.Groups.SingleAsync(x => x.GroupUID == viewModel.GroupUID.Value);
                        company.Name = viewModel.Name;
                        company.OpenDate = viewModel.OpenDate.ToDateTime();
                        company.GroupTypeId = viewModel.TypeId;
                        company.CompaniesHouseNumber = viewModel.CompaniesHouseNumber.Clean();
                        
                        var links = dc.EstablishmentTrusts.Where(x => x.TrustGroupUID == viewModel.GroupUID).ToList();
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
                            var link = new EstablishmentGroup
                            {
                                TrustGroupUID = company.GroupUID,
                                EstablishmentUrn = urn,
                                JoinedDate = DateTime.UtcNow
                            };
                            dc.EstablishmentTrusts.Add(link);
                        }

                        foreach (var urn in urnsToRemove.Cast<int>())
                        {
                            var o = links.FirstOrDefault(x => x.EstablishmentUrn == urn);
                            if (o != null) dc.EstablishmentTrusts.Remove(o);
                        }
                        
                        await dc.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = company.GroupUID });
                    }
                }
            }
            else throw new Exception("Action unrecognised");

            return View(VIEWNAME, viewModel);
        }


        public async Task<ActionResult> Details(int id)
        {
            using (var dc = new ApplicationDbContext())
            {
                var mat = dc.Groups.Include(x => x.GroupType).FirstOrDefault(x => x.GroupUID == id);
                var estabs = dc.EstablishmentTrusts.Include(x => x.Establishment)
                    .Include(x => x.Establishment.EstablishmentType)
                    .Include(x => x.Establishment.HeadTitle)
                    .Where(x => x.Trust.GroupUID == id).ToList();

                LookupDto la = null;
                if(mat.GroupTypeId.OneOfThese(eLookupGroupType.ChildrensCentresCollaboration, eLookupGroupType.ChildrensCentresGroup))
                {
                    la = (await new CachedLookupService().LocalAuthorityGetAllAsync()).First(x => x.Id == estabs.First().Establishment.LocalAuthorityId);
                }

                var gsvc = new GovernorService();
                var historicGovernors = await gsvc.GetHistoricalByGroupUID(id);
                var currentGovernors = await gsvc.GetCurrentByGroupUID(id);

                return View(new MATDetailViewModel(estabs, mat, User.Identity.IsAuthenticated, la)
                {
                    HistoricalGovernors = historicGovernors,
                    Governors = currentGovernors
                });
            }
        }

    }
}