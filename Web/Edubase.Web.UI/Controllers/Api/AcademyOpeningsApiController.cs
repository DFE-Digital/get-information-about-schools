using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    using M = EstablishmentSearchResultModel;

    [MvcAuthorizeRoles(AuthorizedRoles.CanManageAcademyOpenings, AuthorizedRoles.CanManageSecureAcademy16To19Openings)]
    public class AcademyOpeningsApiController : ControllerBase
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

        [Route(
             "api/academy-openings/list/{from:datetime}" +
             "/{to:datetime}/{skip:int}/{take:int}/{establishmentTypeId?}"),
         HttpGet]
        public async Task<object> GetListAsync(DateTime from, DateTime to, int skip, int take,
            string establishmentTypeId = null)
        {
            if (!AcademyUtility.DoesHaveAccessAuthorization(User, establishmentTypeId))
                throw AcademyUtility.GetPermissionDeniedException();

            var estabTypes = await _lookupService.EstablishmentTypesGetAllAsync();
            estabTypes = AcademyUtility.FilterEstablishmentsIfSecureAcademy16To19(estabTypes, establishmentTypeId);

            var apiResult = await _establishmentReadService.SearchAsync(
                new EstablishmentSearchPayload
                {
                    Skip = skip,
                    Take = take,
                    SortBy = eSortBy.NameAlphabeticalAZ,
                    Filters = AcademyUtility.GetEstablishmentSearchFilters(from, to, establishmentTypeId),
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

        [Route("api/academy/{urn:int}"), HttpPost]
        public async Task<IActionResult> SaveAsync(int urn, [FromBody] JsonElement payload)
        {
            if (!payload.TryGetProperty("openDate", out var openDateProperty) || !openDateProperty.TryGetDateTime(out var openingDate))
            {
                return BadRequest("openDate is required and must be a valid date.");
            }

            if (!payload.TryGetProperty("name", out var nameProperty))
            {
                return BadRequest("name is required.");
            }

            var name = nameProperty.GetString();
            var links = await _establishmentReadService.GetLinkedEstablishmentsAsync(urn, User);
            var link = links.FirstOrDefault(e =>
                e.LinkTypeId == (int)eLookupEstablishmentLinkType.ParentOrPredecessor);

            ApiResponse response;
            if (link != null)
            {
                response = await _establishmentWriteService.PartialUpdateAsync(
                    new EstablishmentModel { CloseDate = openingDate.AddDays(-1), Urn = link.Urn },
                    new EstablishmentFieldList { CloseDate = true }, User);

                if (response.HasErrors)
                {
                    return BadRequest(response);
                }
            }

            response = await _establishmentWriteService.PartialUpdateAsync(
                new EstablishmentModel { OpenDate = openingDate, Name = name, Urn = urn },
                new EstablishmentFieldList { OpenDate = true, Name = true }, User);

            if (response.HasErrors)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
