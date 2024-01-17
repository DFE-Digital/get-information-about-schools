using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Common;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models.Tools;

namespace Edubase.Web.UI.Controllers
{
    public class AmalgamateMergeController : Controller
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly ICachedLookupService _lookupService;

        public AmalgamateMergeController(IEstablishmentReadService establishmentReadService,
            IEstablishmentWriteService establishmentWriteService,
            ICachedLookupService lookupService)
        {
            _establishmentReadService = establishmentReadService;
            _establishmentWriteService = establishmentWriteService;
            _lookupService = lookupService;
        }

        [HttpPost, MvcAuthorizeRoles(AuthorizedRoles.CanMergeEstablishments),
         Route("Tools/MergersTool/SelectMergerType"), ValidateAntiForgeryToken]
        public ActionResult SelectMergerType(string mergerType)
        {
            if (mergerType == "Merger")
            {
                return RedirectToAction("MergeEstablishments");
            }

            if (mergerType == "Amalgamation")
            {
                return RedirectToAction("AmalgamateEstablishments");
            }

            ModelState.AddModelError("MergerType", "Please select \"Amalgamation\" or \"Merger\"");
            return View("~/Views/Tools/MergersTool.cshtml");
        }

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanMergeEstablishments),
         Route("Tools/MergersTool/AmalgamateEstablishments")]
        public ActionResult AmalgamateEstablishments()
        {
            return View("~/Views/Tools/Mergers/AmalgamateEstablishments.cshtml");
        }

        [HttpGet, MvcAuthorizeRoles(AuthorizedRoles.CanMergeEstablishments),
         Route("Tools/MergersTool/MergeEstablishments")]
        public ActionResult MergeEstablishments()
        {
            return View("~/Views/Tools/Mergers/MergeEstablishments.cshtml");
        }

        [HttpPost, MvcAuthorizeRoles(AuthorizedRoles.CanMergeEstablishments),
         Route("Tools/MergersTool/MergeEstablishments"), ValidateAntiForgeryToken]
        public async Task<ActionResult> ProcessMergeEstablishmentsAsync(MergeEstablishmentsModel model)
        {
            var viewModel = new MergeEstablishmentsModel();
            var submittedUrns = new List<int>();

            var leadHasErrors = DoesModelStateHaveErrorsForKey("LeadEstablishmentUrn");
            var estab1HasErrors = DoesModelStateHaveErrorsForKey("Establishment1Urn");

            if (!model.LeadEstablishmentUrn.HasValue && !leadHasErrors)
            {
                ModelState.AddModelError(nameof(model.LeadEstablishmentUrn), "Enter the lead establishment URN");
            }

            if (!model.Establishment1Urn.HasValue && !model.Establishment2Urn.HasValue &&
                !model.Establishment3Urn.HasValue && !estab1HasErrors)
            {
                ModelState.AddModelError(nameof(model.Establishment1Urn), "Enter the establishment 1 URN");
            }

            // validate the establishments exist
            if (model.LeadEstablishmentUrn.HasValue)
            {
                var leadEstab =
                    await _establishmentReadService.GetAsync(model.LeadEstablishmentUrn.GetValueOrDefault(), User);

                if (leadEstab.GetResult() == null)
                {
                    ViewData.ModelState.AddModelError(nameof(model.LeadEstablishmentUrn),
                        "The lead establishment URN is invalid");
                }
                else
                {
                    var estabTypes = await _lookupService.EstablishmentTypesGetAllAsync();

                    viewModel.LeadEstablishmentUrn = model.LeadEstablishmentUrn;
                    viewModel.LeadEstablishmentName = leadEstab.GetResult().Name;
                    viewModel.EstablishmentType =
                        estabTypes.FirstOrDefault(t => t.Id == leadEstab.GetResult().TypeId)?.Name;
                    submittedUrns.Add(model.LeadEstablishmentUrn.GetValueOrDefault());
                }
            }

            if (model.Establishment1Urn.HasValue)
            {
                var estab1 =
                    await _establishmentReadService.GetAsync(model.Establishment1Urn.GetValueOrDefault(), User);

                var hasErrors = DoesModelStateHaveErrorsForKey("Establishment1Urn");

                if (estab1.GetResult() == null)
                {
                    if (!hasErrors)
                    {
                        ViewData.ModelState.AddModelError(nameof(model.Establishment1Urn),
                            "The establishment 1 URN is invalid");
                    }
                }
                else
                {
                    viewModel.Establishment1Urn = model.Establishment1Urn;
                    viewModel.Establishment1Name = estab1.GetResult().Name;
                    submittedUrns.Add(model.Establishment1Urn.GetValueOrDefault());
                }
            }

            if (model.Establishment2Urn.HasValue)
            {
                var estab2 =
                    await _establishmentReadService.GetAsync(model.Establishment2Urn.GetValueOrDefault(), User);

                var hasErrors = DoesModelStateHaveErrorsForKey("Establishment2Urn");

                if (estab2.GetResult() == null)
                {
                    if (!hasErrors)
                    {
                        ViewData.ModelState.AddModelError(nameof(model.Establishment2Urn),
                            "The establishment 2 URN is invalid");
                    }
                }
                else
                {
                    viewModel.Establishment2Urn = model.Establishment2Urn;
                    viewModel.Establishment2Name = estab2.GetResult().Name;
                    submittedUrns.Add(model.Establishment2Urn.GetValueOrDefault());
                }
            }

            if (model.Establishment3Urn.HasValue)
            {
                var estab3 =
                    await _establishmentReadService.GetAsync(model.Establishment3Urn.GetValueOrDefault(), User);

                var hasErrors = DoesModelStateHaveErrorsForKey("Establishment3Urn");

                if (estab3.GetResult() == null)
                {
                    if (!hasErrors)
                    {
                        ViewData.ModelState.AddModelError(nameof(model.Establishment3Urn),
                            "The establishment 3 URN is invalid");
                    }
                }
                else
                {
                    viewModel.Establishment3Urn = model.Establishment3Urn;
                    viewModel.Establishment3Name = estab3.GetResult().Name;
                    submittedUrns.Add(model.Establishment3Urn.GetValueOrDefault());
                }
            }

            var duplicates = submittedUrns.GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .ToDictionary(x => x.Key, x => x.Count());


            if (duplicates.ContainsKey(model.LeadEstablishmentUrn.GetValueOrDefault()))
            {
                ViewData.ModelState.AddModelError("LeadEstablishmentUrn", "Duplicate URN. Please correct the URN.");
            }

            if (duplicates.ContainsKey(model.Establishment1Urn.GetValueOrDefault()))
            {
                ViewData.ModelState.AddModelError("Establishment1Urn", "Duplicate URN. Please correct the URN.");
            }

            if (duplicates.ContainsKey(model.Establishment2Urn.GetValueOrDefault()))
            {
                ViewData.ModelState.AddModelError("Establishment2Urn", "Duplicate URN. Please correct the URN.");
            }

            if (duplicates.ContainsKey(model.Establishment3Urn.GetValueOrDefault()))
            {
                ViewData.ModelState.AddModelError("Establishment3Urn", "Duplicate URN. Please correct the URN.");
            }


            return !ModelState.IsValid
                ? View("~/Views/Tools/Mergers/MergeEstablishments.cshtml", model)
                : View("~/Views/Tools/Mergers/ConfirmMerger.cshtml", viewModel);
        }

        [HttpPost, MvcAuthorizeRoles(AuthorizedRoles.CanMergeEstablishments),
         Route("Tools/MergersTool/ConfirmMerger"), ValidateAntiForgeryToken]
        public async Task<ActionResult> ProcessMergeAsync(MergeEstablishmentsModel model)
        {
            if (model.MergeDate == null || model.MergeDate.IsEmpty() || !model.MergeDate.IsValid())
            {
                ViewData.ModelState.AddModelError("MergeDate", "Please enter a valid establishment open date");
                return View("~/Views/Tools/Mergers/ConfirmMerger.cshtml", model);
            }
            if (model.LeadEstablishmentUrn == null)
            {
                ViewData.ModelState.AddModelError("LeadEstablishmentUrn", "Please provide a Lead Establishment URN");
                return View("~/Views/Tools/Mergers/ConfirmMerger.cshtml", model);
            }

            var urns = new List<int>();

            if (model.Establishment1Urn.HasValue)
            {
                model.Establishment1Urn.GetValueOrDefault();
            }

            if (model.Establishment2Urn.HasValue)
            {
                urns.Add(model.Establishment2Urn.GetValueOrDefault());
            }

            if (model.Establishment3Urn.HasValue)
            {
                urns.Add(model.Establishment3Urn.GetValueOrDefault());
            }

            if (urns.Count < 1)
            {
                ViewData.ModelState.AddModelError("Establishment1Urn", "Please provide at least one establishment URN");
                return View("~/Views/Tools/Mergers/ConfirmMerger.cshtml", model);
            }
            else
            {
                var mergeDate = new DateTime(model.MergeDate.Year.GetValueOrDefault(),
                    model.MergeDate.Month.GetValueOrDefault(), model.MergeDate.Day.GetValueOrDefault());

                var result = await _establishmentWriteService.AmalgamateOrMergeAsync(
                    new AmalgamateMergeRequest()
                    {
                        OperationType = AmalgamateMergeRequest.eOperationType.Merge,
                        OperationTypeDescriptor = "merge",
                        LeadEstablishmentUrn = model.LeadEstablishmentUrn,
                        MergeOrAmalgamationDate = mergeDate,
                        UrnsToMerge = urns.ToArray()
                    }, User);

                if (!result.HasErrors)
                {
                    return View("~/Views/Tools/Mergers/MergerComplete.cshtml", model);
                }

                foreach (var err in result.Errors)
                {
                    ViewData.ModelState.AddModelError(err.Fields, err.Message);
                }
            }


            return View("~/Views/Tools/Mergers/ConfirmMerger.cshtml", model);
        }

        [HttpPost, MvcAuthorizeRoles(AuthorizedRoles.CanMergeEstablishments),
         Route("Tools/MergersTool/AmalgamateEstablishments"),
            ValidateAntiForgeryToken]
        public async Task<ActionResult> ProcessAmalgamationEstablishmentsAsync(AmalgamateEstablishmentsModel model)
        {
            var viewModel = new AmalgamateEstablishmentsModel();

            var submittedUrns = new List<int>();
            
            var estab0HasErrors = DoesModelStateHaveErrorsForKey("Establishment0Urn");
            var estab1HasErrors = DoesModelStateHaveErrorsForKey("Establishment1Urn");

            if (!model.Establishment0Urn.HasValue &&!estab0HasErrors)
            {
                ModelState.AddModelError(nameof(model.Establishment0Urn), "Enter the establishment 1 URN");
            }

            if (!model.Establishment1Urn.HasValue && !model.Establishment2Urn.HasValue &&
                !model.Establishment3Urn.HasValue)
            {
                if (!estab1HasErrors)
                {
                    ModelState.AddModelError(nameof(model.Establishment1Urn), "Enter the establishment 2 URN");
                }
            }

            if (model.Establishment0Urn.HasValue)
            {
                var estab =
                    await _establishmentReadService.GetAsync(model.Establishment0Urn.GetValueOrDefault(), User);
                if (estab.GetResult() == null && !estab1HasErrors)
                {
                    ViewData.ModelState.AddModelError(nameof(model.Establishment0Urn),
                        "The establishment 1 URN is invalid");
                }
                else
                {
                    viewModel.Establishment0Urn = model.Establishment0Urn;
                    viewModel.Establishment0Name = estab.GetResult().Name;

                    submittedUrns.Add(model.Establishment0Urn.GetValueOrDefault());
                }
            }

            if (model.Establishment1Urn.HasValue)
            {
                var estab1 =
                    await _establishmentReadService.GetAsync(model.Establishment1Urn.GetValueOrDefault(), User);
                if (estab1.GetResult() == null)
                {
                    ViewData.ModelState.AddModelError(nameof(model.Establishment1Urn),
                        "The establishment 1 URN is invalid");
                }
                else
                {
                    viewModel.Establishment1Urn = model.Establishment1Urn;
                    viewModel.Establishment1Name = estab1.GetResult().Name;

                    submittedUrns.Add(model.Establishment1Urn.GetValueOrDefault());
                }
            }

            if (model.Establishment2Urn.HasValue)
            {
                var estab2 =
                    await _establishmentReadService.GetAsync(model.Establishment2Urn.GetValueOrDefault(), User);
                if (estab2.GetResult() == null)
                {
                    ViewData.ModelState.AddModelError(nameof(model.Establishment2Urn),
                        "The establishment 2 URN is invalid");
                }
                else
                {
                    viewModel.Establishment2Urn = model.Establishment2Urn;
                    viewModel.Establishment2Name = estab2.GetResult().Name;

                    submittedUrns.Add(model.Establishment2Urn.GetValueOrDefault());
                }
            }

            if (model.Establishment3Urn.HasValue)
            {
                var estab3 =
                    await _establishmentReadService.GetAsync(model.Establishment3Urn.GetValueOrDefault(), User);
                if (estab3.GetResult() == null)
                {
                    ViewData.ModelState.AddModelError(nameof(model.Establishment3Urn),
                        "The establishment 3 URN is invalid");
                }
                else
                {
                    viewModel.Establishment3Urn = model.Establishment3Urn;
                    viewModel.Establishment3Name = estab3.GetResult().Name;

                    submittedUrns.Add(model.Establishment3Urn.GetValueOrDefault());
                }
            }

            var duplicates = submittedUrns.GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .ToDictionary(x => x.Key, x => x.Count());


            if (duplicates.ContainsKey(model.Establishment0Urn.GetValueOrDefault()))
            {
                ViewData.ModelState.AddModelError("Establishment0Urn", "Duplicate URN. Please correct the URN.");
            }

            if (duplicates.ContainsKey(model.Establishment1Urn.GetValueOrDefault()))
            {
                ViewData.ModelState.AddModelError("Establishment1Urn", "Duplicate URN. Please correct the URN.");
            }

            if (duplicates.ContainsKey(model.Establishment2Urn.GetValueOrDefault()))
            {
                ViewData.ModelState.AddModelError("Establishment2Urn", "Duplicate URN. Please correct the URN.");
            }

            if (duplicates.ContainsKey(model.Establishment3Urn.GetValueOrDefault()))
            {
                ViewData.ModelState.AddModelError("Establishment3Urn", "Duplicate URN. Please correct the URN.");
            }


            if (!ModelState.IsValid)
            {
                return View("~/Views/Tools/Mergers/AmalgamateEstablishments.cshtml", model);
            }

            viewModel.EstablishmentPhases = (await _lookupService.EducationPhasesGetAllAsync()).ToSelectList();

            viewModel.EstablishmentTypes = (await _lookupService.EstablishmentTypesGetAllAsync()).ToSelectList();

            viewModel.LocalAuthorities = (await _lookupService.LocalAuthorityGetAllAsync()).ToSelectList();

            return View("~/Views/Tools/Mergers/ConfirmAmalgamation.cshtml", viewModel);
        }

        [HttpPost, MvcAuthorizeRoles(AuthorizedRoles.CanMergeEstablishments),
         Route("Tools/MergersTool/ConfirmAmalgamation"),
            ValidateAntiForgeryToken]
        public async Task<ActionResult> ProcessAmalgamationAsync(AmalgamateEstablishmentsModel model)
        {
            if (model.MergeDate.IsEmpty() || model.MergeDate == null || !model.MergeDate.IsValid())
            {
                ViewData.ModelState.AddModelError("MergeDate", "Please enter a valid establishment open date");
            }

            if (string.IsNullOrWhiteSpace(model.NewEstablishmentName))
            {
                ViewData.ModelState.AddModelError("NewEstablishmentName", "Please enter the establishment name");
            }

            if (string.IsNullOrWhiteSpace(model.EstablishmentType))
            {
                ViewData.ModelState.AddModelError("EstablishmentType", "Please select the establishment type");
            }

            if (!model.EstablishmentPhase.HasValue)
            {
                ViewData.ModelState.AddModelError("EstablishmentPhase", "Please select the establishment phase");
            }

            if (string.IsNullOrWhiteSpace(model.LocalAuthorityId))
            {
                ViewData.ModelState.AddModelError("LocalAuthorityId", "Please select a local authority");
            }

            var urns = new List<int>();

            if (model.Establishment0Urn.HasValue)
            {
                urns.Add(model.Establishment0Urn.GetValueOrDefault());
            }

            if (model.Establishment1Urn.HasValue)
            {
                urns.Add(model.Establishment1Urn.GetValueOrDefault());
            }

            if (model.Establishment2Urn.HasValue)
            {
                urns.Add(model.Establishment2Urn.GetValueOrDefault());
            }

            if (model.Establishment3Urn.HasValue)
            {
                urns.Add(model.Establishment3Urn.GetValueOrDefault());
            }

            if (urns.Count < 2)
            {
                ViewData.ModelState.AddModelError("Establishment0Urn", "At least two URNs must be provided");
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Tools/Mergers/ConfirmAmalgamation.cshtml", model);
            }

            model.EstablishmentPhases = (await _lookupService.EducationPhasesGetAllAsync()).ToSelectList();

            model.EstablishmentTypes = (await _lookupService.EstablishmentTypesGetAllAsync()).ToSelectList();

            model.LocalAuthorities = (await _lookupService.LocalAuthorityGetAllAsync()).ToSelectList();

            var mergeDate = new DateTime(model.MergeDate.Year.GetValueOrDefault(),
                model.MergeDate.Month.GetValueOrDefault(), model.MergeDate.Day.GetValueOrDefault());

            var establishmentTypeId = int.TryParse(model.EstablishmentType, out var i) ? i : (int?) null;
            var localAuthorityId = int.TryParse(model.LocalAuthorityId, out var j) ? j : (int?) null;

            var result = await _establishmentWriteService.AmalgamateOrMergeAsync(
                new AmalgamateMergeRequest()
                {
                    OperationType = AmalgamateMergeRequest.eOperationType.Amalgamate,
                    OperationTypeDescriptor = "amalgamate",
                    MergeOrAmalgamationDate = mergeDate,
                    UrnsToMerge = urns.ToArray(),
                    NewEstablishmentPhaseId = model.EstablishmentPhase,
                    NewEstablishmentTypeId = establishmentTypeId,
                    NewEstablishmentName = model.NewEstablishmentName,
                    NewEstablishmentLocalAuthorityId = localAuthorityId,
                }, User);

            if (!result.HasErrors)
            {
                model.NewEstablishmentUrn = result.Response.AmalgamateNewEstablishmentUrn.GetValueOrDefault();
                model.LocalAuthorityName =
                    model.LocalAuthorities.FirstOrDefault(x => x.Value == model.LocalAuthorityId)?.Text;

                model.EstablishmentType = model.EstablishmentTypes
                    .FirstOrDefault(x => x.Value == model.EstablishmentType)?.Text;

                return View("~/Views/Tools/Mergers/AmalgamationComplete.cshtml", model);
            }




            foreach (var err in result.Errors)
            {
                if (err.Fields != null)
                {
                    ViewData.ModelState.AddModelError(err.Fields, err.Message);
                }

            }

            return View("~/Views/Tools/Mergers/ConfirmAmalgamation.cshtml", model);
        }

        private bool DoesModelStateHaveErrorsForKey(string key)
        {
            var hasErrors = false;
            if (ModelState.TryGetValue(key, out var modelState))
            {
                hasErrors = modelState.Errors.Count > 0;
            }
            return hasErrors;
        }
    }
}
