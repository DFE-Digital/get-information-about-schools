using AutoMapper;
using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Data.Identity;
using Edubase.Services;
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
    [Authorize]
    public class EstablishmentController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            using (var dc = ApplicationDbContext.Create())
            {
                var dataModel = dc.Establishments.FirstOrDefault(x => x.Urn == id);
                var viewModel = Mapper.Map<Establishment, ViewModel>(dataModel);

                viewModel.Links = (await dc.Estab2EstabLinks
                    .Include(x => x.LinkedEstablishment)
                    .Where(x => x.Establishment_Urn == id)
                    .Select(x => x)
                    .ToArrayAsync())
                    .Select(x => new LinkedEstab(x)).ToList();

                return View("CreateEdit", viewModel);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ViewModel model)
        {
            if (model.Action == ViewModel.eAction.Save)
            {
                if (ModelState.IsValid)
                {
                    var thereArePendingUpdates = await SaveEstablishment(model);
                    return RedirectToAction("Details", "Schools", new { id = model.Urn.Value, pendingUpdates = thereArePendingUpdates });
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
            return View("CreateEdit", model);
        }

        private static void AddLinkedEstablishment(ViewModel model)
        {
            if (!model.Links.Any(x => x.Urn == model.LinkedUrnToAdd))
            {
                using (var dc = new ApplicationDbContext())
                {
                    var link = new LinkedEstab
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
                List<string> permPropertiesThatChanged = new List<string>();
                permissions.ForEach(p =>
                {
                    var newValue = ReflectionHelper.GetProperty(establishment, p.PropertyName).Clean();
                    var oldValue = ReflectionHelper.GetProperty(dataModel, p.PropertyName).Clean();
                    if (newValue != oldValue)
                    {
                        permPropertiesThatChanged.Add(p.PropertyName);
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

                AddOrRemoveEstablishmentLinks(model, dc);

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

        private void AddOrRemoveEstablishmentLinks(ViewModel model, ApplicationDbContext dc)
        {
            var links = dc.Estab2EstabLinks.Where(x => x.Establishment_Urn == model.Urn).ToList();
            var urnsInDb = links.Select(x => x.LinkedEstablishment_Urn).Cast<int?>().ToArray();
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
                var link = new Estab2Estab
                {
                    Establishment_Urn = model.Urn,
                    LinkedEstablishment_Urn = urn,
                    LinkEstablishedDate = item.LinkDate,
                    LinkName = item.Name,
                    LinkType = item.Type.ToString()
                };
                dc.Estab2EstabLinks.Add(link);
            }

            foreach (var urn in urnsToRemove.Cast<int>())
            {
                var o = links.FirstOrDefault(x => x.LinkedEstablishment_Urn == urn);
                if (o != null) dc.Estab2EstabLinks.Remove(o);
            }

        }

        [HttpGet, Authorize(Roles="Admin,LA")]
        public ActionResult Create() => View("CreateEdit", new ViewModel());


        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,LA")]
        public ActionResult Create([CustomizeValidator(RuleSet = "oncreate")] ViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var dc = ApplicationDbContext.Create())
                {
                    var dataModel = Mapper.Map<Establishment>(model);
                    dc.Establishments.Add(dataModel);
                    dc.SaveChanges();
                    return RedirectToAction("Details", "Schools", new { id = dataModel.Urn });
                }
            }
            else return View("CreateEdit", model);
        }

        




    }
}