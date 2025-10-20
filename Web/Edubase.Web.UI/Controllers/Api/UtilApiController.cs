using System.Threading.Tasks;
using Edubase.Data.Repositories;
using Edubase.Services.Texuna;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    [ApiController]
    [Route("api/save-search-token")]
    public class UtilApiController : ControllerBase
    {
        private readonly IUserPreferenceRepository _userPreferenceRepository;

        public UtilApiController(IUserPreferenceRepository userPreferenceRepository)
        {
            _userPreferenceRepository = userPreferenceRepository;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken] // Use this for API endpoints unless CSRF protection is explicitly required
        public async Task<IActionResult> SaveSearchTokenAsync([FromBody] dynamic payload)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userId = User.GetUserId();
                var prefs = _userPreferenceRepository.Get(userId) ?? new Data.Entity.UserPreference(userId);
                prefs.SavedSearchToken = (string) payload.token;
                await _userPreferenceRepository.UpsertAsync(prefs);
                return NoContent();
            }

            return BadRequest("User not authenticated");
        }
    }
}
