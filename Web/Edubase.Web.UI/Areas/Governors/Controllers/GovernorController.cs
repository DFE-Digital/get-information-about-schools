using Edubase.Services.Enums;
using Edubase.Services.Exceptions;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Models;
using Edubase.Services.Lookup;
using Edubase.Services.Nomenclature;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Exceptions;
using Edubase.Web.UI.Models;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [RouteArea("Governors")]
    public class GovernorController : Controller
    {
        private readonly ICachedLookupService _cachedLookupService;
        private readonly IGovernorsReadService _governorsReadService;
        private readonly NomenclatureService _nomenclatureService;
        private readonly IGovernorsWriteService _governorsWriteService;

        public GovernorController(IGovernorsReadService governorsReadService,
            NomenclatureService nomenclatureService,
            ICachedLookupService cachedLookupService,
            IGovernorsWriteService governorsWriteService)
        {
            _governorsReadService = governorsReadService;
            _nomenclatureService = nomenclatureService;
            _cachedLookupService = cachedLookupService;
            _governorsWriteService = governorsWriteService;
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="groupUId"></param>
        /// <param name="establishmentUrn"></param>
        /// <param name="editMode"></param>
        /// <returns></returns>
        [ChildActionOnly, Route("ViewGovernors")]
        public ActionResult ViewOrEdit(int? groupUId, int? establishmentUrn, bool? editMode)
        {
            // KHD Hack: Async child actions are not supported; but we have an async stack, so we have to wrap the async calls in an sync wrapper.  Hopefully won't deadlock.
            // Need to use ASP.NET Core really now; that supports ViewComponents which are apparently the solutions.
            var result = Task.Run(async () =>
            {
                using (MiniProfiler.Current.Step("Retrieving Governors Details"))
                {
                    var domainModel = await _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, User);
                    var viewModel = new GovernorsGridViewModel(domainModel, editMode.GetValueOrDefault(), groupUId, establishmentUrn, _nomenclatureService);
                    if (viewModel.EditMode)
                    {
                        viewModel.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();
                    }
                    return View("ViewEdit", viewModel);
                }

            }).Result;

            return result;
        }


        /// <summary>
        /// GET
        /// </summary>
        /// <param name="id"></param>
        /// <param name="role"></param>
        /// <param name="gid"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        [ChildActionOnly, Route("AddEdit"), HttpGet]
        public ActionResult AddEdit(int? groupUId, int? establishmentUrn, eLookupGovernorRole? role, int? gid, bool? replace)
        {
            // KHD Hack: Async child actions are not supported; but we have an async stack, so we have to wrap the async calls in an sync wrapper.  Hopefully won't deadlock.
            // Need to use ASP.NET Core really now; that supports ViewComponents which are apparently the solutions.
            var result = Task.Run(async () =>
            {
                if (role == null && gid == null) throw new EdubaseException("Role was not supplied and no Governor ID was supplied");
                var viewModel = new CreateEditGovernorViewModel() { GroupUId = groupUId, EstablishmentUrn = establishmentUrn };
                if (gid.HasValue)
                {
                    var model = await _governorsReadService.GetGovernorAsync(gid.Value);
                    role = (eLookupGovernorRole)model.RoleId.Value;

                    if (replace.GetValueOrDefault())
                    {
                        viewModel.ReplaceGovernorViewModel.AppointmentEndDate = new DateTimeViewModel(model.AppointmentEndDate);
                        viewModel.ReplaceGovernorViewModel.GID = gid;
                        viewModel.ReplaceGovernorViewModel.Name = model.GetFullName();
                    }
                    else
                    {
                        viewModel.AppointingBodyId = model.AppointingBodyId;
                        viewModel.AppointmentEndDate = new DateTimeViewModel(model.AppointmentEndDate);
                        viewModel.AppointmentStartDate = new DateTimeViewModel(model.AppointmentStartDate);
                        viewModel.DOB = new DateTimeViewModel(model.DOB);
                        viewModel.EmailAddress = model.EmailAddress;

                        viewModel.GovernorTitle = model.Person_Title;
                        viewModel.FirstName = model.Person_FirstName;
                        viewModel.MiddleName = model.Person_MiddleName;
                        viewModel.LastName = model.Person_LastName;

                        viewModel.PreviousTitle = model.PreviousPerson_Title;
                        viewModel.PreviousFirstName = model.PreviousPerson_FirstName;
                        viewModel.PreviousMiddleName = model.PreviousPerson_MiddleName;
                        viewModel.PreviousLastName = model.PreviousPerson_LastName;

                        viewModel.GID = model.Id;
                        viewModel.NationalityId = !string.IsNullOrWhiteSpace(model.Nationality) ? (await _cachedLookupService.NationalitiesGetAllAsync()).SingleOrDefault(x => x.Name == model.Nationality)?.Id : null as int?;
                        viewModel.TelephoneNumber = model.TelephoneNumber;
                        viewModel.PostCode = model.PostCode;

                        viewModel.EstablishmentUrn = model.EstablishmentUrn;
                        viewModel.GroupUId = model.GroupUID;
                    }
                }

                viewModel.GovernorRoleName = _nomenclatureService.GetGovernorRoleName(role.Value);
                viewModel.GovernorRole = role.Value;
                await PopulateSelectLists(viewModel);
                viewModel.DisplayPolicy = _governorsReadService.GetEditorDisplayPolicy(role.Value);

                ModelState.Clear();

                return View(viewModel);


            }).Result;

            return result;
        }


        [ChildActionOnly]
        [Route("AddEdit"), HttpPost, ActionName("AddEditPost")]
        public ActionResult AddEdit(CreateEditGovernorViewModel viewModel)
        {
            // KHD Hack: Async child actions are not supported; but we have an async stack, so we have to wrap the async calls in an sync wrapper.  Hopefully won't deadlock.
            // Need to use ASP.NET Core really now; that supports ViewComponents which are apparently the solutions.
            var result = Task.Run(async () =>
            {
                await PopulateSelectLists(viewModel);
                viewModel.DisplayPolicy = _governorsReadService.GetEditorDisplayPolicy(viewModel.GovernorRole);

                if (ModelState.IsValid)
                {
                    if (viewModel.ReplaceGovernorViewModel.GID.HasValue)
                    {
                        var governorBeingReplaced = await _governorsReadService.GetGovernorAsync(viewModel.ReplaceGovernorViewModel.GID.Value);
                        governorBeingReplaced.AppointmentEndDate = viewModel.ReplaceGovernorViewModel.AppointmentEndDate.ToDateTime();
                        await _governorsWriteService.SaveAsync(governorBeingReplaced, User);
                    }

                    viewModel.GID = await _governorsWriteService.SaveAsync(new GovernorModel
                    {
                        AppointingBodyId = viewModel.AppointingBodyId,
                        AppointmentEndDate = viewModel.AppointmentEndDate.ToDateTime(),
                        AppointmentStartDate = viewModel.AppointmentStartDate.ToDateTime(),
                        DOB = viewModel.DOB.ToDateTime(),
                        EmailAddress = viewModel.EmailAddress,
                        GroupUID = viewModel.GroupUId,
                        EstablishmentUrn = viewModel.EstablishmentUrn,
                        Nationality = viewModel.NationalityId.HasValue ? viewModel.Nationalities.FirstOrDefault(x => x.Value == viewModel.NationalityId.ToString())?.Text : null as string,
                        Id = viewModel.GID,
                        Person_FirstName = viewModel.FirstName,
                        Person_MiddleName = viewModel.MiddleName,
                        Person_LastName = viewModel.LastName,
                        Person_Title = viewModel.GovernorTitle,
                        PreviousPerson_FirstName = viewModel.PreviousFirstName,
                        PreviousPerson_MiddleName = viewModel.PreviousMiddleName,
                        PreviousPerson_LastName = viewModel.PreviousLastName,
                        PreviousPerson_Title = viewModel.PreviousTitle,
                        PostCode = viewModel.PostCode,
                        RoleId = (int)viewModel.GovernorRole,
                        TelephoneNumber = viewModel.TelephoneNumber
                    }, User);

                    ModelState.Clear();

                    return (ActionResult) RedirectToAction("EditGovernance", new { id = 1 /*viewModel.GroupUId */ });
                }

                return View("AddEdit", viewModel);

            }).Result;

            return result;

            
        }

        [HttpPost, ChildActionOnly]
        [Route("SaveGovernor")]
        public async Task<ActionResult> SaveGovernor(GovernorsGridViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Action == "Save") // retire selected governor with the chosen appt. end date
                {
                    var domainModel = await _governorsReadService.GetGovernorAsync(viewModel.RemovalGid.Value);
                    domainModel.AppointmentEndDate = viewModel.RemovalAppointmentEndDate.ToDateTime().Value;
                    await _governorsWriteService.SaveAsync(domainModel, User);
                }
                else if (viewModel.Action == "Remove") // mark the governor record as deleted
                {
                    await _governorsWriteService.DeleteAsync(viewModel.RemovalGid.Value, User);
                }
                else throw new InvalidParameterException($"The parameter for action is invalid: '{viewModel.Action}'");

                return null;//RedirectToAction(nameof(EditGovernance), new { id = viewModel.Id });
            }
            else return null; //await EditGovernance(viewModel.Id.Value, viewModel.RemovalGid);
        }

        private async Task PopulateSelectLists(CreateEditGovernorViewModel viewModel)
        {
            viewModel.AppointingBodies = (await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()).ToSelectList(viewModel.AppointingBodyId);
            viewModel.Nationalities = (await _cachedLookupService.NationalitiesGetAllAsync()).ToSelectList(viewModel.NationalityId);
            viewModel.Titles = viewModel.GetTitles();
            viewModel.PreviousTitles = viewModel.GetTitles();
        }
    }
}