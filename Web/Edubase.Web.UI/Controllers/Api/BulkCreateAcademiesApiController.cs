using System;
using System.Threading.Tasks;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Lookup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    [ApiController]
    [Route("api/bulk-create-academies")]
    [Authorize(Roles = "AP_AOS,ROLE_BACKOFFICE,EFADO")]
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

        /// <summary>
        /// POST api/bulk-create-academies
        /// </summary>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ProcessRequestAsync([FromBody] NewAcademyRequest[] payload)
        {
            var result = await _establishmentWriteService.BulkCreateAcademies(payload, User);
            return result.HasErrors ? BadRequest(result) : Ok(result);
        }

        /// <summary>
        /// POST api/bulk-create-academies/validate
        /// </summary>
        [HttpPost("validate")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ProcessValidateRequestAsync([FromBody] NewAcademyRequest[] payload)
        {
            var result = await _establishmentWriteService.ValidateBulkCreateAcademies(payload, User);
            return result.Success ? Ok(result.GetResponse()) : BadRequest(result.Errors);
        }

        /// <summary>
        /// GET api/bulk-create-academies/progress/{id}
        /// </summary>
        [HttpGet("progress/{id}")]
        public async Task<IActionResult> ProcessProgressRequestAsync(Guid id)
        {
            var result = await _establishmentWriteService.GetBulkCreateAcademiesProgress(id, User);
            return result.Success ? Ok(result.GetResponse()) : BadRequest(result.Errors);
        }
    }
}
