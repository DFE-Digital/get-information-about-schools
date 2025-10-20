using Edubase.Services.Approvals;
using Edubase.Services.Approvals.Models;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Web.UI.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    public class BulkCreateAcademiesApiController : ControllerBase
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly ICachedLookupService _lookupService;

        public BulkCreateAcademiesApiController(IEstablishmentReadService establishmentReadService,
            IEstablishmentWriteService establishmentWriteService,
            ICachedLookupService lookupService)
        {
            _establishmentReadService = establishmentReadService;
            _lookupService = lookupService;
            _establishmentWriteService = establishmentWriteService;
        }

        [Route("api/bulk-create-academies"), HttpPost, HttpAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO)]
        public async Task<IActionResult> ProcessRequestAsync(NewAcademyRequest[] payload)
        {
            var result = await _establishmentWriteService.BulkCreateAcademies(payload, User);
            if (!result.HasErrors) return Ok(result);
            else return Content(HttpStatusCode.BadRequest, result);
        }

        [Route("api/bulk-create-academies/validate"), HttpPost, HttpAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO)]
        public async Task<IActionResult> ProcessValidateRequestAsync(NewAcademyRequest[] payload)
        {
            var result = await _establishmentWriteService.ValidateBulkCreateAcademies(payload, User);
            if (result.Success) return Ok(result.GetResponse());
            else return Content(HttpStatusCode.BadRequest, result.Errors);
        }

        [Route("api/bulk-create-academies/progress/{id}"), HttpGet, HttpAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO)]
        public async Task<IActionResult> ProcessProgressRequestAsync(Guid id)
        {
            var result = await _establishmentWriteService.GetBulkCreateAcademiesProgress(id, User);
            if (result.Success) return Ok(result.GetResponse());
            else return Content(HttpStatusCode.BadRequest, result.Errors);
        }
    }
}