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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSearchTokenAsync([FromBody] SaveSearchTokenRequest payload)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.GetUserId();
                var prefs = _userPreferenceRepository.Get(userId) ?? new Data.Entity.UserPreference(userId);
                prefs.SavedSearchToken = payload.Token;
                await _userPreferenceRepository.UpsertAsync(prefs);
                return NoContent();
            }

            return BadRequest("User not authenticated");
        }
    }

    public class SaveSearchTokenRequest
    {
        public string Token { get; set; }
    }
}
