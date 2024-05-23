using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Security;
using Edubase.Web.UI.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    public class AmalgamateMergeApiController : ApiController
    {
        private readonly IEstablishmentWriteService _establishmentWriteService;
        private IEnumerable<string> tokenHeaderValues;

        public AmalgamateMergeApiController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [System.Web.Http.Route("api/amalgamate-merge"), System.Web.Http.HttpPost, HttpAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO, EdubaseRoles.SOU, EdubaseRoles.IEBT)]
        [ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> ProcessRequestAsync(AmalgamateMergeRequest payload)
        {

            var tokenHeaderString = "requestverificationtoken";

            if (Request.Headers.TryGetValues(tokenHeaderString, out tokenHeaderValues))
            {
                // The header exists
                var tokenHeaderList = tokenHeaderValues.ToList();
                if (tokenHeaderList.Count == 0)
                {
                    // header present - no values
                }
                else if (tokenHeaderList.Count > 1)
                {
                    // multiple values
                }
                else
                {
                    // exactly one value
                }
            }
            else
            {
                // header is not present
            }

            var result = await _establishmentWriteService.AmalgamateOrMergeAsync(payload, User);
            return !result.HasErrors ? Ok(result) : (IHttpActionResult) Content(HttpStatusCode.BadRequest, result);
        }
    }
}
