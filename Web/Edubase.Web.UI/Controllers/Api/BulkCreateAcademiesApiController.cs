using System;
using System.Threading.Tasks;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    [ApiController]
    [Route("api/bulk-create-academies")]
    public class BulkCreateAcademiesApiController : ControllerBase
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly ICachedLookupService _lookupService;

        public BulkCreateAcademiesApiController(
            IEstablishmentReadService establishmentReadService,
            IEstablishmentWriteService establishmentWriteService,
            ICachedLookupService lookupService)
        {
            _establishmentReadService = establishmentReadService;
            _lookupService = lookupService;
            _establishmentWriteService = establishmentWriteService;
        }

        [HttpPost]
        [HttpAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO)]
        public async Task<IActionResult> ProcessRequestAsync(NewAcademyRequest[] payload)
        {
            var result = await _establishmentWriteService.BulkCreateAcademies(payload, User);
            return result.HasErrors ? BadRequest(result) : Ok(result);
        }

        [HttpPost("validate")]
        [HttpAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO)]
        public async Task<IActionResult> ProcessValidateRequestAsync(NewAcademyRequest[] payload)
        {
            var result = await _establishmentWriteService.ValidateBulkCreateAcademies(payload, User);
            return result.Success ? Ok(result.GetResponse()) : BadRequest(result.Errors);
        }

        [HttpGet("progress/{id}")]
        [HttpAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO)]
        public async Task<IActionResult> ProcessProgressRequestAsync(Guid id)
        {
            var result = await _establishmentWriteService.GetBulkCreateAcademiesProgress(id, User);
            return result.Success ? Ok(result.GetResponse()) : BadRequest(result.Errors);
        }
    }
}
