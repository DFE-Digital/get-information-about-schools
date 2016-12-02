using AutoMapper;
using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Data.Identity;
using Edubase.Services;
using Edubase.Services.Establishments;
using Edubase.Services.Groups;
using Edubase.Services.Security;
using Edubase.Web.UI.Models;
using FluentValidation.Mvc;
using Microsoft.ServiceBus.Messaging;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModel = Edubase.Web.UI.Models.CreateEditEstablishmentModel;

namespace Edubase.Web.UI.Controllers
{
    public class EstablishmentController : Controller
    {
        private IEstablishmentReadService _establishmentReadService;
        private IGroupReadService _groupReadService;

        public EstablishmentController(IEstablishmentReadService establishmentReadService, IGroupReadService groupReadService)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }

        [HttpGet, Authorize]
        public async Task<ActionResult> Edit(int id)
        {
            using (var dc = ApplicationDbContext.Create())
            {
                var dataModel = dc.Establishments.FirstOrDefault(x => x.Urn == id);
                var viewModel = Mapper.Map<Establishment, ViewModel>(dataModel);

                viewModel.Links = (await dc.EstablishmentLinks
                    .Include(x => x.LinkedEstablishment)
                    .Where(x => x.EstablishmentUrn == id)
                    .Select(x => x)
                    .ToArrayAsync())
                    .Select(x => new LinkedEstabViewModel(x)).ToList();

                return View(viewModel);
            }
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<ActionResult> Edit(ViewModel model)
        {
            if (model.Action == ViewModel.eAction.Save)
            {
                if (ModelState.IsValid)
                {
                    var thereArePendingUpdates = await SaveEstablishment(model);
                    if(thereArePendingUpdates) return RedirectToAction("Details", "Establishment", new { id = model.Urn.Value, pendingUpdates = true });
                    else return RedirectToAction("Details", "Establishment", new { id = model.Urn.Value });
                }
            }
            else
            {
                if (model.Action == ViewModel.eAction.FindEstablishment && model.LinkedSearchUrn.HasValue)
                {
                    ModelState.Clear();
                    model.LinkedEstabNameToAdd = new EstablishmentService().GetName(model.LinkedSearchUrn.Value);
                    model.LinkedUrnToAdd = model.LinkedSearchUrn;
                }
                else if (model.Action == ViewModel.eAction.AddLinkedSchool)
                {
                    if (ModelState.IsValid)
                    {
                        AddLinkedEstablishment(model);
                        ModelState.Clear();
                    }
                }
                else if (model.Action == ViewModel.eAction.RemoveLinkedSchool)
                {
                    ModelState.Clear();
                    if (model.LinkedItemPositionToRemove.HasValue)
                        model.Links.RemoveAt(model.LinkedItemPositionToRemove.Value);
                }
                model.ScrollToLinksSection = true;
            }
            return View(model);
        }

        private void AddLinkedEstablishment(ViewModel model)
        {
            if (!model.Links.Any(x => x.Urn == model.LinkedUrnToAdd))
            {
                using (var dc = new ApplicationDbContext())
                {
                    var link = new LinkedEstabViewModel
                    {
                        LinkDate = model.LinkedDateToAdd.ToDateTime(),
                        Name = model.LinkedEstabNameToAdd,
                        Type = model.LinkTypeToAdd.ToString(),
                        Urn = model.LinkedUrnToAdd
                    };
                    model.Links.Insert(0, link);
                    model.LinkedSearchUrn = model.LinkedUrnToAdd = null;
                    model.LinkedEstabNameToAdd = null;
                }
            }
        }

        private async Task<bool> SaveEstablishment(ViewModel model)
        {
            using (var dc = ApplicationDbContext.Create())
            {
                Establishment dataModel2 = null;
                using (var dc2 = ApplicationDbContext.Create()) dataModel2 = dc2.Establishments.FirstOrDefault(x => x.Urn == model.Urn);
                var dataModel = await dc.Establishments.FirstOrDefaultAsync(x => x.Urn == model.Urn);

                var role = Roles.RestrictiveRoles.FirstOrDefault(x => User.IsInRole(x)) ?? Roles.Academy;
                var permissions = await dc.Permissions.Where(x => x.RoleName == role).ToArrayAsync();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ContactDetailsViewModel, ContactDetail>();
                    cfg.CreateMap<AddressViewModel, Address>();
                    cfg.CreateMap<DateTimeViewModel, DateTime?>().ConvertUsing<DateTimeTypeConverter>();

                    var map = cfg.CreateMap<ViewModel, Establishment>();
                    permissions.Where(x => !x.PropertyName.Contains("_"))
                        .ForEach(p => map.ForMember(p.PropertyName, opt => opt.Ignore()));
                });

                var mapper = config.CreateMapper();
                var estabTemp = mapper.Map(model, dataModel2);
                var changes = ReflectionHelper.DetectChanges(estabTemp, dataModel, typeof(Address), typeof(ContactDetail));
                mapper.Map(model, dataModel);

                var establishment = Mapper.Map<ViewModel, Establishment>(model);
                var permPropertiesThatChanged = new List<ChangeDescriptor>();
                permissions.ForEach(p =>
                {
                    var newValue = ReflectionHelper.GetProperty(establishment, p.PropertyName).Clean();
                    var oldValue = ReflectionHelper.GetProperty(dataModel, p.PropertyName).Clean();

                    if (newValue != oldValue)
                    {
                        permPropertiesThatChanged.Add(new ChangeDescriptor(p.PropertyName, newValue, oldValue));
                        dc.EstablishmentApprovalQueue.Add(new EstablishmentApprovalQueue
                        {
                            Urn = dataModel.Urn,
                            Name = p.PropertyName,
                            NewValue = newValue,
                            OldValue = oldValue,
                            OriginatorUserId = ((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier).Value
                        });
                    }
                });

                new EstablishmentService().AddChangeHistory(dataModel.Urn, dc, null,
                    ((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier).Value,
                    DateTime.UtcNow, changes.ToArray());

                await AddOrRemoveEstablishmentLinks(model, dc);

                await dc.SaveChangesAsync();

                if (changes.Count > 0)
                {
                    new SmtpClient().Send("kris.dyson@contentsupport.co.uk", ConfigurationManager.AppSettings["DataOwnerEmailAddress"], "Establishment data changed",
                        $"For Establishment URN: {dataModel.Urn}, the following has changed: \r\n" + string.Join("\r\n", changes));

                    await new BusMessagingService().SendEstablishmentUpdateMessageAsync(dataModel);
                }

                if (permPropertiesThatChanged.Count > 0)
                {
                    new SmtpClient().Send("kris.dyson@contentsupport.co.uk", ConfigurationManager.AppSettings["DataOwnerEmailAddress"], "Establishment data changes require approval",
                        $"For Establishment URN: {dataModel.Urn}, the following has changed and requires approval: \r\n" + string.Join("\r\n", permPropertiesThatChanged));
                    return true;
                }
            }
            return false;
        }

        private async Task AddOrRemoveEstablishmentLinks(ViewModel model, ApplicationDbContext dc)
        {
            var svc = new CachedLookupService();

            var linksInDb = dc.EstablishmentLinks.Where(x => x.EstablishmentUrn == model.Urn).ToList();
            var urnsInDb = linksInDb.Select(x => x.LinkedEstablishmentUrn).Cast<int?>().ToArray();
            var urnsInModel = model.Links.Select(x => x.Urn).Cast<int?>().ToArray();

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
                var item = model.Links.Where(x => x.Urn == urn).First();
                var link = new EstablishmentLink
                {
                    EstablishmentUrn = model.Urn,
                    LinkedEstablishmentUrn = urn,
                    LinkEstablishedDate = item.LinkDate,
                    LinkName = item.Name,
                    LinkTypeId = (await svc.EstablishmentLinkTypesGetAllAsync()).Single(x => x.Name == item.Type).Id
                };
                dc.EstablishmentLinks.Add(link);

                var oppositeLinkType = (item.Type.Equals(ViewModel.eLinkType.Successor.ToString()) ? ViewModel.eLinkType.Predecessor : ViewModel.eLinkType.Successor);
                var oppositeLinkTypeName = oppositeLinkType.ToString();
                var oppositeLink = await dc.EstablishmentLinks.FirstOrDefaultAsync(x => x.EstablishmentUrn == urn && x.LinkedEstablishmentUrn == model.Urn && x.LinkType.Name == oppositeLinkTypeName);
                if (oppositeLink == null)
                {
                    oppositeLink = new EstablishmentLink
                    {
                        EstablishmentUrn = urn,
                        LinkedEstablishmentUrn = model.Urn,
                        LinkEstablishedDate = item.LinkDate,
                        LinkName = model.Name,
                        LinkTypeId = (await svc.EstablishmentLinkTypesGetAllAsync()).Single(x => x.Name == oppositeLinkType.ToString()).Id
                    };
                    dc.EstablishmentLinks.Add(oppositeLink);
                }
            }

            foreach (var urn in urnsToRemove.Cast<int>())
            {
                var linkDataModel = linksInDb.FirstOrDefault(x => x.LinkedEstablishmentUrn == urn);
                if (linkDataModel != null) dc.EstablishmentLinks.Remove(linkDataModel);

                var oppositeLinkType = (linkDataModel.LinkType.Equals(ViewModel.eLinkType.Successor.ToString()) ? ViewModel.eLinkType.Predecessor : ViewModel.eLinkType.Successor).ToString();
                var oppositeLink = await dc.EstablishmentLinks.FirstOrDefaultAsync(x => x.EstablishmentUrn == urn && x.LinkedEstablishmentUrn == model.Urn && x.LinkType.Name == oppositeLinkType);
                if (oppositeLink != null) dc.EstablishmentLinks.Remove(oppositeLink);
            }

        }

        [HttpGet, Authorize]
        public ActionResult Create() => View(new ViewModel());
        
        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public ActionResult Create([CustomizeValidator(RuleSet = "oncreate")] ViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var dc = ApplicationDbContext.Create())
                {
                    var dataModel = Mapper.Map<Establishment>(model);

                    var svc = new EstablishmentService();
                    var pol = svc.GetEstabNumberEntryPolicy(dataModel.TypeId.Value, dataModel.EducationPhaseId.Value);
                    if(pol == EstablishmentService.EstabNumberEntryPolicy.SystemGenerated)
                    {
                        dataModel.EstablishmentNumber = svc.GenerateEstablishmentNumber(dataModel.TypeId.Value, dataModel.EducationPhaseId.Value, dataModel.LocalAuthorityId.Value);
                    }

                    dc.Establishments.Add(dataModel);
                    dc.SaveChanges();
                    return RedirectToAction("Details", "Establishment", new { id = dataModel.Urn });
                }
            }
            else return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id, bool? pendingUpdates)
        {
            var viewModel = new EstablishmentDetailViewModel()
            {
                ShowPendingMessage = pendingUpdates.GetValueOrDefault(),
                IsUserLoggedOn = User.Identity.IsAuthenticated
            };

            viewModel.Establishment = await _establishmentReadService.GetAsync(id, User);
            if (viewModel.Establishment == null) return HttpNotFound();

            viewModel.LinkedEstablishments = (await _establishmentReadService.GetLinkedEstablishments(id)).Select(x => new LinkedEstabViewModel(x));

            if (User.Identity.IsAuthenticated)
            {
                viewModel.ChangeHistory = await _establishmentReadService.GetChangeHistoryAsync(id, 20, User);
                viewModel.UserHasPendingApprovals = new ApprovalService().Any(User as ClaimsPrincipal, id);
            }

            viewModel.Group = await _groupReadService.GetByEstablishmentUrnAsync(id);

            var gsvc = new GovernorService();
            viewModel.HistoricalGovernors = await gsvc.GetHistoricalByUrn(id);
            viewModel.Governors = await gsvc.GetCurrentByUrn(id);

            viewModel.DisplayPolicy = _establishmentReadService.GetDisplayPolicy(User, viewModel.Establishment, viewModel.Group);

            return View(viewModel);
        }



    }
}