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
using System.Web.Http;

namespace Edubase.Web.UI.Controllers.Api
{
    public class AmalgamateMergeApiController : ApiController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private readonly ICachedLookupService _lookupService;

        public AmalgamateMergeApiController(IEstablishmentReadService establishmentReadService,
            IEstablishmentWriteService establishmentWriteService,
            ICachedLookupService lookupService)
        {
            _establishmentReadService = establishmentReadService;
            _lookupService = lookupService;
            _establishmentWriteService = establishmentWriteService;
        }

        [Route("api/amalgamate-merge"), HttpPost, HttpAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO, EdubaseRoles.SOU, EdubaseRoles.IEBT)]
        public async Task<IHttpActionResult> ProcessRequestAsync(AmalgamateMergeRequest payload)
        {
            var result = await _establishmentWriteService.AmalgamateOrMergeAsync(payload, User);
            if (!result.HasErrors) return Ok(result);
            else return Content(HttpStatusCode.BadRequest, result);
        }
    }
}