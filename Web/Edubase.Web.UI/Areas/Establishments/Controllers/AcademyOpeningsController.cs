using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Services.Establishments;
using Edubase.Web.UI.Helpers;
using System.Web.Mvc;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Filters;
using Microsoft.Ajax.Utilities;

namespace Edubase.Web.UI.Areas.Establishments.Controllers
{
    using M = EstablishmentSearchResultModel;

    [RouteArea("Establishments"), System.Web.Mvc.RoutePrefix("manage"), System.Web.Mvc.Route("{action=index}"),
     MvcAuthorizeRoles(AuthorizedRoles.CanManageAcademyOpenings, AuthorizedRoles.CanManageSecure16To19AcademyOpenings)]
    public class AcademyOpeningsController : Controller
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly ICachedLookupService _lookupService;

        public AcademyOpeningsController(IEstablishmentReadService establishmentReadService,
            IEstablishmentWriteService establishmentWriteService,
            ICachedLookupService lookupService)
        {
            _establishmentReadService = establishmentReadService;
            _lookupService = lookupService;
            _establishmentWriteService = establishmentWriteService;
        }

        [HttpGet, Route("16-19-secure-academy-openings", Name = "Manage16To19SecureAcademyOpenings")]
        public Task<ActionResult> Manage16To19SecureAcademyOpenings(int skip = 0, string sortBy = "OpenDate-desc")
        {
            //secure 16-19 academy establishment type Id is 46
            return Task.FromResult<ActionResult>(RedirectToAction(nameof(ManageAcademyOpenings),
                new
                {
                    skip,
                    sortBy,
                    isSecure16To19User = SecureAcademyUtility.EncryptValue(true.ToString()),
                    establishmentTypeId = SecureAcademyUtility.EncryptValue("46")
                }));
        }

        [HttpGet, Route("academy-openings", Name = "ManageAcademyOpenings")]
        public async Task<ActionResult> ManageAcademyOpenings(int skip = 0, string sortBy = "OpenDate-desc",
            string isSecure16To19User = null, string establishmentTypeId = null)
        {
            var take = 50;
            var now = DateTime.Now;

            var from = new DateTime(now.Year, now.Month, 1);

            var to = from.AddYears(30);

            var property = typeof(EditAcademyOpeningViewModel).GetProperty(sortBy);

            var isUserSecure16To19 = !string.IsNullOrWhiteSpace(isSecure16To19User)
                                     && bool.Parse(SecureAcademyUtility.DecryptValue(isSecure16To19User));

            if (!string.IsNullOrWhiteSpace(establishmentTypeId))
                establishmentTypeId = SecureAcademyUtility.DecryptValue(establishmentTypeId);

            var estabTypes = await _lookupService.EstablishmentTypesGetAllAsync();
            estabTypes = SecureAcademyUtility.FilterEstablishmentsByEstablishmentTypeId
                (estabTypes, establishmentTypeId, isUserSecure16To19);

            var result = await _establishmentReadService.SearchAsync(
                new EstablishmentSearchPayload
                {
                    Skip = skip,
                    Take = take,
                    SortBy = eSortBy.NameAlphabeticalAZ,
                    Filters =
                        SecureAcademyUtility.GetEstablishmentSearchFilters(
                            new GetEstabSearchFiltersParam(from, to, establishmentTypeId, isUserSecure16To19)),
                    Select = new List<string>
                    {
                        nameof(M.Name),
                        nameof(M.Urn),
                        nameof(M.TypeId),
                        nameof(M.OpenDate),
                        nameof(M.PredecessorName),
                        nameof(M.PredecessorUrn)
                    }
                }, User);


            var academyOpenings = new List<EditAcademyOpeningViewModel>();
            foreach (var x in result.Items)
            {
                int preUrn = 0;
                int.TryParse(x.PredecessorUrn, out preUrn);

                academyOpenings.Add(new EditAcademyOpeningViewModel()
                {
                    Urn = (int) x.Urn,
                    EstablishmentName = x.Name,
                    EstablishmentType =
                        x.TypeId.HasValue ? estabTypes.FirstOrDefault(t => t.Id == x.TypeId)?.Name : null,
                    OpeningDate = new UI.Models.DateTimeViewModel(x.OpenDate),
                    OpenDate = x.OpenDate.GetValueOrDefault(),
                    PredecessorName = x.PredecessorName,
                    PredecessorUrn = preUrn.ToString()
                });
            }

            switch (sortBy)
            {
                case "OpenDate-desc":
                    academyOpenings.Sort((x, y) => x.OpenDate.CompareTo(y.OpenDate));
                    break;
                case "OpenDate-asc":
                    academyOpenings.Sort((x, y) => y.OpenDate.CompareTo(x.OpenDate));
                    break;
                case "Urn-asc":
                    academyOpenings.Sort((x, y) => x.Urn.CompareTo(y.Urn));
                    break;
                case "Urn-desc":
                    academyOpenings.Sort((x, y) => y.Urn.CompareTo(x.Urn));
                    break;
                case "EstablishmentName-asc":
                    academyOpenings.Sort((x, y) => x.EstablishmentName.CompareTo(y.EstablishmentName));
                    break;
                case "EstablishmentName-desc":
                    academyOpenings.Sort((x, y) => y.EstablishmentName.CompareTo(x.EstablishmentName));
                    break;

                case "EstablishmentType-asc":
                    academyOpenings.Sort((x, y) => x.EstablishmentType.CompareTo(y.EstablishmentType));
                    break;
                case "EstablishmentType-desc":
                    academyOpenings.Sort((x, y) => y.EstablishmentType.CompareTo(x.EstablishmentType));
                    break;
                case "PredecessorUrn-asc":
                    academyOpenings.Sort((x, y) => x.PredecessorUrn.CompareTo(y.PredecessorUrn));
                    break;
                case "PredecessorUrn-desc":
                    academyOpenings.Sort((x, y) => y.PredecessorUrn.CompareTo(x.PredecessorUrn));
                    break;
                case "PredecessorName-asc":
                    academyOpenings.Sort(delegate(EditAcademyOpeningViewModel x, EditAcademyOpeningViewModel y)
                    {
                        return y.PredecessorName == null && y.PredecessorName == null
                            ? 0
                            : x.PredecessorName == null
                                ? 1
                                : y.PredecessorName == null
                                    ? -1
                                    : x.PredecessorName.CompareTo(y.PredecessorName);
                    });
                    break;
                case "PredecessorName-desc":
                    academyOpenings.Sort(delegate(EditAcademyOpeningViewModel x, EditAcademyOpeningViewModel y)
                    {
                        return y.PredecessorName == null && y.PredecessorName == null
                            ? 0
                            : x.PredecessorName == null
                                ? -1
                                : y.PredecessorName == null
                                    ? 1
                                    : y.PredecessorName.CompareTo(x.PredecessorName);
                    });
                    break;
            }


            var vm = new ManageAcademyOpeningsViewModel()
            {
                AcademyOpenings =
                    new PaginatedResult<EditAcademyOpeningViewModel>(skip, take, result.Count, academyOpenings),
                Items = academyOpenings,
                PageTitle = SecureAcademyUtility.GetAcademyOpeningPageTitle(establishmentTypeId, isUserSecure16To19)
            };
            vm.Count = result.Count;
            vm.Skip = skip;
            return View("Index", vm);
        }

        [HttpGet, Route("edit-academy-opening/{urn}", Name = "EditAcademyOpening")]
        public async Task<ActionResult> EditAcademyOpening(int? urn)
        {
            if (!urn.HasValue)
            {
                return HttpNotFound();
            }

            var result = await _establishmentReadService.GetAsync((int) urn, User);

            var estabTypes = await _lookupService.EstablishmentTypesGetAllAsync();

            var links = await _establishmentReadService.GetLinkedEstablishmentsAsync((int) urn, User);
            var link = links.FirstOrDefault(e =>
                e.LinkTypeId == (int) eLookupEstablishmentLinkType.ParentOrPredecessor);

            var establishment = result.GetResult();

            var viewModel = new EditAcademyOpeningViewModel()
            {
                Urn = (int) urn,
                EstablishmentName = establishment.Name,
                EstablishmentType =
                    establishment.TypeId.HasValue
                        ? estabTypes.FirstOrDefault(t => t.Id == establishment.TypeId)?.Name
                        : null,
                PredecessorUrn = link?.Urn.GetValueOrDefault().ToString(),
                PredecessorName = link?.EstablishmentName,
                OpeningDate = new UI.Models.DateTimeViewModel(establishment.OpenDate)
            };

            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, EdubaseAuthorize,
         Route("edit-academy-opening/{urn}", Name = "SaveAcademyOpening")]
        public async Task<ActionResult> SaveAcademyOpening(EditAcademyOpeningViewModel viewModel)
        {
            if (viewModel.EstablishmentName.IsNullOrWhiteSpace())
            {
                ModelState.AddModelError("EstablishmentName", "Please enter the establishment name");
            }

            if (!ModelState.IsValid)
            {
                return View("EditAcademyOpening", viewModel);
            }

            var openingDate = viewModel.OpeningDate.ToDateTime().GetValueOrDefault();
            var links = await _establishmentReadService.GetLinkedEstablishmentsAsync(viewModel.Urn, User);
            var link = links.FirstOrDefault(e =>
                e.LinkTypeId == (int) eLookupEstablishmentLinkType.ParentOrPredecessor);

            ApiResponse response;
            if (link != null)
            {
                response = await _establishmentWriteService.PartialUpdateAsync(
                    new EstablishmentModel { CloseDate = openingDate.AddDays(-1), Urn = link.Urn },
                    new EstablishmentFieldList { CloseDate = true }, User);

                if (response.HasErrors)
                {
                    foreach (var e in response.Errors)
                    {
                        ModelState.AddModelError(e.Fields ?? string.Empty, e.GetMessage());
                    }
                }
            }

            response = await _establishmentWriteService.PartialUpdateAsync(
                new EstablishmentModel
                {
                    OpenDate = openingDate, Name = viewModel.EstablishmentName, Urn = viewModel.Urn
                }, new EstablishmentFieldList { OpenDate = true, Name = true }, User);

            if (response.HasErrors)
            {
                foreach (var e in response.Errors)
                {
                    ModelState.AddModelError(e.Fields ?? string.Empty, e.GetMessage());
                }
            }

            return RedirectToAction("ManageAcademyOpenings");
        }

        [HttpGet, Route("search-academies", Name = "SearchAcademyOpenings")]
        public async Task<ActionResult> SearchAcademyOpenings(int? urn, bool? isSearching)
        {
            if (urn.HasValue)
            {
                var result = await _establishmentReadService.GetAsync((int) urn, User);
                if (result.ReturnValue == null)
                {
                    ModelState.AddModelError("Urn", "Please enter a valid URN");
                    return View("SearchAcademyOpenings", new EditAcademyOpeningViewModel());
                }

                var estabTypes = await _lookupService.EstablishmentTypesGetAllAsync();
                var links = await _establishmentReadService.GetLinkedEstablishmentsAsync((int) urn, User);
                var link = links.FirstOrDefault(e =>
                    e.LinkTypeId == (int) eLookupEstablishmentLinkType.ParentOrPredecessor);

                var establishment = result.GetResult();
                if (establishment.StatusId != (int) eLookupEstablishmentStatus.ProposedToOpen)
                {
                    ModelState.AddModelError("Urn", "Please enter a valid URN");
                    return View("SearchAcademyOpenings", new EditAcademyOpeningViewModel());
                }


                var viewModel = new EditAcademyOpeningViewModel()
                {
                    Urn = (int) urn,
                    EstablishmentName = establishment.Name,
                    EstablishmentType =
                        establishment.TypeId.HasValue
                            ? estabTypes.FirstOrDefault(t => t.Id == establishment.TypeId)?.Name
                            : null,
                    PredecessorUrn = link?.Urn.GetValueOrDefault().ToString(),
                    PredecessorName = link?.EstablishmentName,
                    OpeningDate = new UI.Models.DateTimeViewModel(establishment.OpenDate),
                    OpenDate = establishment.OpenDate ?? DateTime.Now
                };
                return View("SearchAcademyOpenings", viewModel);
            }

            if (isSearching ?? false)
            {
                ModelState.AddModelError("Urn", "Please enter a valid URN");
            }

            return View("SearchAcademyOpenings", new EditAcademyOpeningViewModel());
        }
    }
}
