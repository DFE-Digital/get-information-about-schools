using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Edubase.Web.UI.Controllers.Api
{
    using M = EstablishmentSearchResultModel;

    [MvcAuthorizeRoles(AuthorizedRoles.CanManageAcademyOpenings, AuthorizedRoles.CanManageSecure16To19AcademyOpenings)]
    public class AcademyOpeningsApiController : ApiController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly ICachedLookupService _lookupService;

        public AcademyOpeningsApiController(IEstablishmentReadService establishmentReadService,
            IEstablishmentWriteService establishmentWriteService,
            ICachedLookupService lookupService)
        {
            _establishmentReadService = establishmentReadService;
            _lookupService = lookupService;
            _establishmentWriteService = establishmentWriteService;
        }

        /// <summary>
        /// Treating myself in this one to a really nice URL. I deserve it. And so does Jon.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="establishmentTypeId"></param>
        /// <returns></returns>
        [Route(
             "api/academy-openings/list/{from:datetime}/{to:datetime}" +
             "/{skip:int}/{take:int}/{establishmentTypeId?}"),
         HttpGet]
        public async Task<dynamic> GetListAsync(DateTime from, DateTime to, int skip, int take,
            string establishmentTypeId = null)
        {
            if (!AcademyUtility.DoesHaveAccessAuthorization(User, establishmentTypeId))
                throw AcademyUtility.GetAccessViolationException();

            var roleName = AcademyUtility.GetSecureAcademy16To19Role(User);
            var isUserSecure16To19 = AcademyUtility.IsUserSecureAcademy16To19User(roleName);
            establishmentTypeId =
                AcademyUtility.GetDecryptedEstablishmentTypeId(establishmentTypeId, isUserSecure16To19);

            var estabTypes = await _lookupService.EstablishmentTypesGetAllAsync();
            estabTypes =
                AcademyUtility.FilterEstablishmentsByEstablishmentTypeId(estabTypes, establishmentTypeId,
                    isUserSecure16To19);

            var apiResult = await _establishmentReadService.SearchAsync(
                new EstablishmentSearchPayload
                {
                    Skip = skip,
                    Take = take,
                    SortBy = eSortBy.NameAlphabeticalAZ,
                    Filters = AcademyUtility.GetEstablishmentSearchFilters
                        (new GetEstabSearchFiltersParam(from, to, establishmentTypeId, isUserSecure16To19)),
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


            return new
            {
                Items = apiResult.Items.Select(x => new
                {
                    x.Urn,
                    x.Name,
                    EstablishmentType =
                        x.TypeId.HasValue ? estabTypes.FirstOrDefault(t => t.Id == x.TypeId)?.Name : null,
                    OpeningDate = x.OpenDate,
                    DisplayDate = x.OpenDate?.ToString("d MMMM yyyy"),
                    x.PredecessorName,
                    x.PredecessorUrn,
                }).OrderBy(x => x.OpeningDate),
                apiResult.Count
            };
        }

        /// <summary>
        /// POST api/academy/{urn}
        /// Takes a payload with openDate and Name properties.
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        [Route("api/academy/{urn:int}"), HttpPost]
        public async Task<HttpResponseMessage> SaveAsync(int urn, [FromBody] dynamic payload)
        {
            DateTime openingDate = payload.openDate;
            var links = await _establishmentReadService.GetLinkedEstablishmentsAsync(urn, User);
            var link = links.FirstOrDefault(e =>
                e.LinkTypeId == (int) eLookupEstablishmentLinkType.ParentOrPredecessor);

            ApiResponse response;
            if (link != null)
            {
                response = await _establishmentWriteService.PartialUpdateAsync(
                    new EstablishmentModel { CloseDate = openingDate.AddDays(-1), Urn = link.Urn },
                    new EstablishmentFieldList { CloseDate = true }, User);

                if (response.HasErrors) return Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }

            response = await _establishmentWriteService.PartialUpdateAsync(
                new EstablishmentModel { OpenDate = openingDate, Name = payload.name, Urn = urn },
                new EstablishmentFieldList { OpenDate = true, Name = true }, User);

            if (response.HasErrors) return Request.CreateResponse(HttpStatusCode.BadRequest, response);

            else return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
