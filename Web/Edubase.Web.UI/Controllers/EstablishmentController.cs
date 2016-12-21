using AutoMapper;
using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Data.Identity;
using Edubase.Services;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Enums;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Governors;
using Edubase.Services.Groups;
using Edubase.Services.Security;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Establishments;
using FluentValidation.Mvc;
using Microsoft.ServiceBus.Messaging;
using MoreLinq;
using StackExchange.Profiling;
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
        private IMapper _mapper;
        private ILAESTABService _laEstabService;
        private IEstablishmentWriteService _establishmentWriteService;

        public EstablishmentController(IEstablishmentReadService establishmentReadService, 
            IGroupReadService groupReadService, IMapper mapper, 
            ILAESTABService laEstabService,
            IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
            _mapper = mapper;
            _laEstabService = laEstabService;
            _establishmentWriteService = establishmentWriteService;
        }

        [HttpGet, EdubaseAuthorize]
        public async Task<ActionResult> Edit(int id)
        {
            var domainModel = (await _establishmentReadService.GetAsync(id, User)).GetResult();
            return View(_mapper.Map<ViewModel>(domainModel));
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize]
        public async Task<ActionResult> Edit(ViewModel model)
        {
            if (model.Action == ViewModel.eAction.Save)
            {
                if (ModelState.IsValid)
                {
                    await _establishmentWriteService.SaveAsync(_mapper.Map<EstablishmentModel>(model), User);
                    return RedirectToAction("Details", "Establishment", new { id = model.Urn.Value });
                }
            }
            else
            {

                //if (model.Action == ViewModel.eAction.FindEstablishment && model.LinkedSearchUrn.HasValue)
                //{
                //    ModelState.Clear();
                //    model.LinkedEstabNameToAdd = new EstablishmentService().GetName(model.LinkedSearchUrn.Value);
                //    model.LinkedUrnToAdd = model.LinkedSearchUrn;
                //}
                //else if (model.Action == ViewModel.eAction.AddLinkedSchool)
                //{
                //    if (ModelState.IsValid)
                //    {
                //        AddLinkedEstablishment(model);
                //        ModelState.Clear();
                //    }
                //}
                //else if (model.Action == ViewModel.eAction.RemoveLinkedSchool)
                //{
                //    ModelState.Clear();
                //    if (model.LinkedItemPositionToRemove.HasValue)
                //        model.Links.RemoveAt(model.LinkedItemPositionToRemove.Value);
                //}
                //model.ScrollToLinksSection = true;
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

        [HttpGet, EdubaseAuthorize]
        public ActionResult Create() => View(new ViewModel());
        
        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize]
        public ActionResult Create([CustomizeValidator(RuleSet = "oncreate")] ViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var dc = ApplicationDbContext.Create())
                {
                    var dataModel = _mapper.Map<Establishment>(model);
                    var pol = _laEstabService.GetEstabNumberEntryPolicy(dataModel.TypeId.Value, dataModel.EducationPhaseId.Value);
                    if(pol == EstabNumberEntryPolicy.SystemGenerated)
                    {
                        dataModel.EstablishmentNumber = _laEstabService.GenerateEstablishmentNumber(dataModel.TypeId.Value, dataModel.EducationPhaseId.Value, dataModel.LocalAuthorityId.Value);
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

            using (MiniProfiler.Current.Step("Retrieving establishment"))
            {
                var result = await _establishmentReadService.GetAsync(id, User);
                if (!result.Success) return HttpNotFound();
                viewModel.Establishment = result.ReturnValue;
            }
            

            using (MiniProfiler.Current.Step("Retrieving LinkedEstablishments"))
            {
                viewModel.LinkedEstablishments = (await _establishmentReadService.GetLinkedEstablishments(id)).Select(x => new LinkedEstabViewModel(x));
            }
            

            if (User.Identity.IsAuthenticated)
            {
                using (MiniProfiler.Current.Step("Retrieving ChangeHistory"))
                    viewModel.ChangeHistory = await _establishmentReadService.GetChangeHistoryAsync(id, 20, User);

                using (MiniProfiler.Current.Step("Retrieving UserHasPendingApprovals flag"))
                    viewModel.UserHasPendingApprovals = new ApprovalService().Any(User as ClaimsPrincipal, id);
            }

            using (MiniProfiler.Current.Step("Retrieving Group record"))
                viewModel.Group = await _groupReadService.GetByEstablishmentUrnAsync(id);

            var gsvc = new GovernorsReadService();

            using (MiniProfiler.Current.Step("Retrieving HistoricalGovernors"))
                viewModel.HistoricalGovernors = await gsvc.GetHistoricalByUrn(id);

            using (MiniProfiler.Current.Step("Retrieving Governors"))
                viewModel.Governors = await gsvc.GetCurrentByUrn(id);

            using (MiniProfiler.Current.Step("Retrieving DisplayPolicy"))
                viewModel.DisplayPolicy = _establishmentReadService.GetDisplayPolicy(User, viewModel.Establishment, viewModel.Group);

            using (MiniProfiler.Current.Step("Retrieving TabDisplayPolicy"))
                viewModel.TabDisplayPolicy = new TabDisplayPolicy(viewModel.Establishment, User);

            return View(viewModel);
        }



    }
}