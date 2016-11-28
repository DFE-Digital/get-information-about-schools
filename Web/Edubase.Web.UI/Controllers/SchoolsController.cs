﻿using System.Linq;
using System.Web.Mvc;
using Edubase.Data.Entity;
using System.Dynamic;
using System.Data.Entity;
using Edubase.Web.UI.Identity;
using System;
using System.Collections.Generic;
using Edubase.Web.UI.Models;
using Edubase.Common;
using Edubase.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using Edubase.Services.Security;

namespace Edubase.Web.UI.Controllers
{
    public class SchoolsController : Controller
    {
        private readonly ISchoolPermissions _schoolPermissions;
        
        public SchoolsController(ISchoolPermissions schoolPermissions)
        {
            _schoolPermissions = schoolPermissions;
        }

        // GET: School
        public ActionResult Index()
        {
            var accessibleSchoolIds = _schoolPermissions.GetAccessibleSchoolIds().ToArray();

            if (accessibleSchoolIds.Length == 1)
            {
                return RedirectToAction("Details", new {id = accessibleSchoolIds.Single()});
            }

            using (var dc = new ApplicationDbContext())
            {
                var model = accessibleSchoolIds.Select(id =>
                {
                    dynamic o = new ExpandoObject();
                    o.SCHNAME = dc.Establishments.FirstOrDefault(x => x.Urn == id)?.Name;
                    o.id = id;
                    return o;
                });
                return View(model);
            }
        }

        public async Task<ActionResult> Details(int id, bool? pendingUpdates)
        {
            var viewModel = new EstablishmentDetailViewModel(User)
            {
                ShowPendingMessage = pendingUpdates.GetValueOrDefault(),
                IsUserLoggedOn = User.Identity.IsAuthenticated
            };

            using (var dc = new ApplicationDbContext())
            {
                var model = await dc.Establishments
                    .Include(x => x.AdmissionsPolicy)
                    .Include(x => x.Diocese)
                    .Include(x => x.EducationPhase)
                    .Include(x => x.EstablishmentType)
                    .Include(x => x.Gender)
                    .Include(x => x.HeadTitle)
                    .Include(x => x.LocalAuthority)
                    .Include(x => x.ProvisionBoarding)
                    .Include(x => x.ProvisionNursery)
                    .Include(x => x.ProvisionOfficialSixthForm)
                    .Include(x => x.ProvisionSpecialClasses)
                    .Include(x => x.ReasonEstablishmentClosed)
                    .Include(x => x.ReasonEstablishmentOpened)
                    .Include(x => x.ReligiousCharacter)
                    .Include(x => x.ReligiousEthos)
                    .Include(x => x.Status)
                    .Include(x => x.EstablishmentTypeGroup)
                    .Include(x => x.Inspectorate)
                    .Include(x => x.Section41Approved)
                    .Include(x => x.SEN1)
                    .Include(x => x.SEN2)
                    .Include(x => x.SEN3)
                    .Include(x => x.SEN4)
                    .Include(x => x.TeenageMothersProvision)
                    .Include(x => x.ChildcareFacilities)
                    .Include(x => x.PRUSEN)
                    .Include(x => x.PRUEBD)
                    .Include(x => x.PruFulltimeProvision)
                    .Include(x => x.PruEducatedByOthers)
                    .Include(x => x.TypeOfResourcedProvision)
                    .Include(x => x.GovernmentOfficeRegion)
                    .Include(x => x.AdministrativeDistrict)
                    .Include(x => x.AdministrativeWard)
                    .Include(x => x.ParliamentaryConstituency)
                    .Include(x => x.UrbanRural)
                    .Include(x => x.GSSLA)
                    .Include(x => x.CASWard)
                    .Include(x => x.MSOA)
                    .Include(x => x.LSOA)
                    .Include(x => x.FurtherEducationType)
                    .Include(x => x.RSCRegion)
                    .Include(x => x.BSOInspectorate)
                    .FirstOrDefaultAsync(x => x.Urn == id);

                if (model == null) return HttpNotFound();

                viewModel.Establishment = model;
                if(User.Identity.IsAuthenticated) viewModel.ChangeHistory = await new EstablishmentService().GetChangeHistoryAsync(id, dc);

                viewModel.LinkedEstablishments = (await dc.EstablishmentLinks
                    .Include(x => x.LinkedEstablishment)
                    .Include(x => x.LinkType)
                    .Where(x => x.EstablishmentUrn == id).ToArrayAsync())
                    .Select(x => new LinkedEstabViewModel(x)).ToArray();

                viewModel.Group = (await dc.EstablishmentTrusts.Include(x => x.Trust).FirstOrDefaultAsync(x => x.EstablishmentUrn == id))?.Trust;

                var pred = new SecurityService().GetEditEstablishmentPermission(User);
                viewModel.UserCanEdit = pred.CanEdit(model.Urn, model.TypeId, null, model.LocalAuthorityId, model.EstablishmentTypeGroupId);


                var gsvc = new GovernorService();
                viewModel.HistoricalGovernors = await gsvc.GetHistoricalByUrn(id);
                viewModel.Governors = await gsvc.GetCurrentByUrn(id);
                

                if (User.Identity.IsAuthenticated)
                {
                    var pending = await dc.EstablishmentApprovalQueue.Where(x => x.Urn == id && x.IsApproved == false && x.IsDeleted == false && x.IsRejected == false).ToListAsync();
                    if (pending.Any())
                    {
                        foreach (var item in pending)
                        {
                            var change = new PendingChangeViewModel() { DataField = item.Name };
                            var number = item.NewValue.ToInteger();
                            switch (item.Name)
                            {
                                case "LocalAuthorityId":
                                    change.NewValue = dc.LocalAuthorities.FirstOrDefault(x => x.Id == number)?.Name;
                                    change.OldValue = model.LocalAuthority?.ToString();
                                    break;
                                case "HeadTitleId":
                                    change.NewValue = dc.LookupHeadTitles.FirstOrDefault(x => x.Id == number)?.Name;
                                    change.OldValue = model.HeadTitle?.ToString();
                                    break;
                                case "GenderId":
                                    change.NewValue = dc.LookupGenders.FirstOrDefault(x => x.Id == number)?.Name;
                                    change.OldValue = model.Gender?.ToString();
                                    break;
                                case "EducationPhaseId":
                                    change.NewValue = dc.LookupEducationPhases.FirstOrDefault(x => x.Id == number)?.Name;
                                    change.OldValue = model.EducationPhase?.ToString();
                                    break;
                                case "AdmissionsPolicyId":
                                    change.NewValue = dc.LookupAdmissionsPolicies.FirstOrDefault(x => x.Id == number)?.Name;
                                    change.OldValue = model.AdmissionsPolicy?.ToString();
                                    break;
                                case "StatusId":
                                    change.NewValue = dc.LookupEstablishmentStatuses.FirstOrDefault(x => x.Id == number)?.Name;
                                    change.OldValue = model.Status?.ToString();
                                    break;
                                default:
                                    change.NewValue = item.NewValue;
                                    change.OldValue = ReflectionHelper.GetProperty(model, item.Name);
                                    break;
                            }
                            viewModel.PendingChanges.Add(change);
                        }
                    }
                }

                if (viewModel.IsUserLoggedOn)
                {
                    viewModel.UserHasPendingApprovals = new ApprovalService().Any(User as ClaimsPrincipal, id);
                }
                return View(viewModel);
            }
        }
    }
}