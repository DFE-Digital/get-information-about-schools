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
using Edubase.Web.UI.Filters;
using Edubase.Data.DbContext;
using Edubase.Services.Governors;
using Edubase.Common.Cache;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using MoreLinq;
using Edubase.Web.UI.Models.Validators;
using FluentValidation;
using FluentValidation.Mvc;
using System.Collections.Generic;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Group")]
    public class GroupController : Controller
    {
        const string VIEWNAME = "CreateEdit";
        ICachedLookupService _cachedLookupService;
        ISecurityService _securityService;
        private IGovernorsReadService _governorsReadService;

        public GroupController(ICachedLookupService cachedLookupService, ISecurityService securityService,
            IGovernorsReadService governorsReadService)
        {
            _cachedLookupService = cachedLookupService;
            _securityService = securityService;
            _governorsReadService = governorsReadService;
        }

        [EdubaseAuthorize, Route(nameof(SearchCompaniesHouse))]
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

        [HttpGet, EdubaseAuthorize, Route(nameof(Create) + "/{id}")]
        public async Task<ActionResult> Create(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return HttpNotFound();

            var companyProfile = await new TrustService().SearchByCompaniesHouseNumber(id);
            var groupTypes = await GetGroupTypes();

            var vm = new CreateGroupModel(companyProfile.Items.First(), groupTypes);
            using (var dc = new ApplicationDbContext())
            {
                vm.TrustExists = await dc.Groups.AnyAsync(x => x.CompaniesHouseNumber == id);
            }

            return View("Create", vm);
        }
        
        [HttpPost, EdubaseAuthorize, Route(nameof(Create) + "/{id}")]
        public async Task<ActionResult> Create(CreateGroupModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var id = await new TrustService().CreateAsync(User as ClaimsPrincipal, viewModel.CompaniesHouseNumber, viewModel.TypeId.Value);
                return RedirectToAction("Details", new { id });
            }
            else
            {
                viewModel.GroupTypes = await GetGroupTypes(viewModel.TypeId);
                return View(viewModel);
            }
        }

        private async Task<IEnumerable<SelectListItem>> GetGroupTypes(int? typeId = null)
        {
            return (await _cachedLookupService.GroupTypesGetAllAsync())
                .Where(x => x.Name.IndexOf("trust", StringComparison.OrdinalIgnoreCase) > -1)
                .ToSelectList(typeId);
        }

        [HttpGet, EdubaseAuthorize, Route("Edit/{id:int}")]
        public async Task<ActionResult> Edit(int id)
        {
            var viewModel = new CreateEditGroupModel();
            using (var dc = new ApplicationDbContext())
            {
                var company = await dc.Groups.FirstOrDefaultAsync(x => x.GroupUID == id);
                viewModel.GroupUID = company.GroupUID;
                viewModel.GroupId = company.GroupId;
                viewModel.Name = company.Name;
                viewModel.TypeId = company.GroupTypeId;
                viewModel.OpenDate = company.OpenDate;
                viewModel.CompaniesHouseNumber = company.CompaniesHouseNumber;
                viewModel.Address = company.Address;

                if (company.GroupTypeId.HasValue)
                    viewModel.TypeName = (await _cachedLookupService.GroupTypesGetAllAsync()).FirstOrDefault(x => x.Id == company.GroupTypeId)?.Name;
            
                viewModel.Establishments = (await dc.EstablishmentGroups
                    .Include(x => x.Establishment)
                    .Include(x => x.Establishment.EstablishmentType)
                    .Include(x => x.Establishment.HeadTitle)
                    .Where(x => x.Group.GroupUID == company.GroupUID)
                    .ToArrayAsync())
                    .Select(x => new GroupEstabViewModel(x.Establishment, x.JoinedDate)).ToList();
            }
            return View(VIEWNAME, viewModel);
        }
        
        [HttpPost, EdubaseAuthorize, Route("Edit/{id:int}")]
        public async Task<ActionResult> Edit(CreateEditGroupModel viewModel)
        {
            if (viewModel.Action == "Search")
            {
                ModelState.Clear();
                var urn = viewModel.SearchURN.ToInteger();

                using (var dc = new ApplicationDbContext())
                    viewModel.EstablishmentName = dc.Establishments.Where(x => x.Urn == urn)
                        .Select(x => x.Name).FirstOrDefault();

                if (viewModel.EstablishmentName != null) viewModel.EstablishmentUrn = urn;
                else viewModel.EstablishmentNotFound = true;
            }
            else if (viewModel.Action == "Add")
            {
                var validator = new DateTimeViewModelValidator();
                var results = validator.Validate(viewModel.JoinedDate);
                if (!results.IsValid) viewModel.AddModelError(x => x.JoinedDate, "The date specified is not valid", ModelState);
                else
                {
                    ModelState.Clear();
                    if (!viewModel.Establishments.Any(x => x.Urn == viewModel.EstablishmentUrn))
                    {
                        using (var dc = new ApplicationDbContext())
                        {
                            var estab = new GroupEstabViewModel(await dc.Establishments
                                .Include(x => x.HeadTitle)
                                .Include(x => x.EstablishmentType)
                                .FirstOrDefaultAsync(x => x.Urn == viewModel.EstablishmentUrn), viewModel.JoinedDate.ToDateTime());
                            viewModel.Establishments.Insert(0, estab);
                            viewModel.SearchURN = string.Empty;
                            viewModel.EstablishmentUrn = null;
                            viewModel.EstablishmentName = null;
                        }
                    }
                }
            }
            else if (viewModel.Action.StartsWith("edit-", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.Clear();
                var urn = int.Parse(viewModel.Action.Split('-')[1]);
                var m = viewModel.Establishments.Single(x => x.Urn == urn);
                m.EditMode = true;
                m.JoinedDateEditable = new DateTimeViewModel(m.JoinedDate);
            }
            else if (viewModel.Action.Equals("Cancel", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.Clear();
                viewModel.Establishments.ForEach(x => x.EditMode = false);
            }
            else if (viewModel.Action.Equals("SaveJoinedDate", StringComparison.OrdinalIgnoreCase))
            {
                var model = viewModel.Establishments.First(x => x.EditMode == true);
                var i = viewModel.Establishments.IndexOf(model);

                var validator = new DateTimeViewModelValidator();
                var results = validator.Validate(model.JoinedDateEditable);

                if (!results.IsValid)
                {
                    viewModel.AddModelError(x => x.Establishments[i].JoinedDateEditable, "The date specified is not valid", ModelState);
                }
                else
                {
                    ModelState.Clear();
                    model.JoinedDate = model.JoinedDateEditable.ToDateTime();
                    viewModel.Establishments.ForEach(x => x.EditMode = false);
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
                        var group = await dc.Groups.SingleAsync(x => x.GroupUID == viewModel.GroupUID.Value);
                        group.GroupId = viewModel.GroupId.Clean();

                        if (!_securityService.GetEditGroupPermission(User).CanEdit(group.GroupUID, group.GroupTypeId))
                        {
                            return new HttpUnauthorizedResult("Edit permission denied");
                        }

                        var links = dc.EstablishmentGroups.Where(x => x.GroupUID == viewModel.GroupUID).ToList();
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
                            var joinedDate = viewModel.Establishments.FirstOrDefault(x => x.Urn == urn)?.JoinedDate;

                            var link = new EstablishmentGroup
                            {
                                GroupUID = group.GroupUID,
                                EstablishmentUrn = urn,
                                JoinedDate = joinedDate
                            };
                            dc.EstablishmentGroups.Add(link);
                        }

                        foreach (var urn in urnsToRemove.Cast<int>())
                        {
                            var o = links.FirstOrDefault(x => x.EstablishmentUrn == urn);
                            if (o != null) dc.EstablishmentGroups.Remove(o);
                        }

                        var urnsToUpdate = urnsInDb.Intersect(urnsInModel);
                        foreach (var urn in urnsToUpdate)
                        {
                            var o = links.FirstOrDefault(x => x.EstablishmentUrn == urn);
                            o.JoinedDate = viewModel.Establishments.FirstOrDefault(x => x.Urn == urn)?.JoinedDate;
                        }

                        await dc.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = group.GroupUID });
                    }
                }
            }
            else throw new Exception("Action unrecognised");

            return View(VIEWNAME, viewModel);
        }

        [Route("Details/{id}")]
        public async Task<ActionResult> Details(int id)
        {
            using (var dc = new ApplicationDbContext())
            {
                var group = dc.Groups.Include(x => x.GroupType).FirstOrDefault(x => x.GroupUID == id);
                var estabs = dc.EstablishmentGroups.Include(x => x.Establishment)
                    .Include(x => x.Establishment.EstablishmentType)
                    .Include(x => x.Establishment.HeadTitle)
                    .Where(x => x.Group.GroupUID == id).ToList();

                LookupDto la = null;
                if(group.GroupTypeId.OneOfThese(eLookupGroupType.ChildrensCentresCollaboration, 
                    eLookupGroupType.ChildrensCentresGroup))
                {
                    la = (await _cachedLookupService.LocalAuthorityGetAllAsync())
                        .First(x => x.Id == estabs.First().Establishment.LocalAuthorityId);
                }

                var historicGovernors = await _governorsReadService.GetHistoricalByGroupUID(id);
                var currentGovernors = await _governorsReadService.GetCurrentByGroupUID(id);

                var canUserEdit = _securityService.GetEditGroupPermission(User).CanEdit(id, group.GroupTypeId);

                return View(new MATDetailViewModel(estabs, group, User.Identity.IsAuthenticated, la)
                {
                    HistoricalGovernors = historicGovernors,
                    Governors = currentGovernors,
                    CanUserEdit = canUserEdit
                });
            }
        }

    }
}