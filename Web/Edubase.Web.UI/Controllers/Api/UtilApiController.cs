using Edubase.Data.Repositories;
using Edubase.Services.Texuna;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    public class UtilApiController : ApiController
    {
        private readonly IUserPreferenceRepository _userPreferenceRepository;

        public UtilApiController(IUserPreferenceRepository userPreferenceRepository)
        {
            _userPreferenceRepository = userPreferenceRepository;
        }

        [System.Web.Http.Route("api/save-search-token"), System.Web.Http.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> SaveSearchTokenAsync(dynamic payload)
        {
            if (User.Identity.IsAuthenticated)
            {
                var prefs = _userPreferenceRepository.Get(User.GetUserId()) ?? new Data.Entity.UserPreference(User.GetUserId());
                prefs.SavedSearchToken = (string) payload.token;
                await _userPreferenceRepository.UpsertAsync(prefs);
                return StatusCode(System.Net.HttpStatusCode.NoContent);
            }
            else
            {
                return BadRequest("User not authenticated");
            }
        }
    }
}
