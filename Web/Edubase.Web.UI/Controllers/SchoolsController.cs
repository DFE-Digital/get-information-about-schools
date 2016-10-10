using System.Linq;
using System.Web.Mvc;
using Edubase.Data.Entity;
using System.Dynamic;
using System.Data.Entity;
using Edubase.Web.UI.Identity;
using System;
using System.Collections.Generic;
using Edubase.Web.UI.Models;
using Edubase.Common;

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

        public ActionResult Details(int id, bool? pendingUpdates)
        {
            var viewModel = new EstablishmentDetailViewModel
            {
                ShowPendingMessage = pendingUpdates.GetValueOrDefault(),
                IsUserLoggedOn = User.Identity.IsAuthenticated
            };

            using (var dc = new ApplicationDbContext())
            {
                var model = dc.Establishments
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
                    .Include(x => x.EstablishmentType)
                    .FirstOrDefault(x => x.Urn == id);

                viewModel.Establishment = model;
                
                if (User.Identity.IsAuthenticated)
                {
                    var pending = dc.EstablishmentApprovalQueue.Where(x => x.Urn == id).ToList();
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
                                    change.NewValue = dc.HeadTitles.FirstOrDefault(x => x.Id == number)?.Name;
                                    change.OldValue = model.HeadTitle?.ToString();
                                    break;
                                case "GenderId":
                                    change.NewValue = dc.Genders.FirstOrDefault(x => x.Id == number)?.Name;
                                    change.OldValue = model.Gender?.ToString();
                                    break;
                                case "EducationPhaseId":
                                    change.NewValue = dc.EducationPhases.FirstOrDefault(x => x.Id == number)?.Name;
                                    change.OldValue = model.EducationPhase?.ToString();
                                    break;
                                case "AdmissionsPolicyId":
                                    change.NewValue = dc.AdmissionsPolicies.FirstOrDefault(x => x.Id == number)?.Name;
                                    change.OldValue = model.AdmissionsPolicy?.ToString();
                                    break;
                                case "StatusId":
                                    change.NewValue = dc.EstablishmentStatuses.FirstOrDefault(x => x.Id == number)?.Name;
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

                return View(viewModel);
            }
        }
    }
}