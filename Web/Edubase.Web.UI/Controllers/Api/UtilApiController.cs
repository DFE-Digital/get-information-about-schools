using Edubase.Data.Repositories;
using Edubase.Services.Texuna;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    public class UtilApiController : ControllerBase
    {
        private readonly IUserPreferenceRepository _userPreferenceRepository;

        public UtilApiController(IUserPreferenceRepository userPreferenceRepository)
        {
            _userPreferenceRepository = userPreferenceRepository;
        }

        [Route("api/save-search-token"), HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSearchTokenAsync(dynamic payload)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.GetUserId();
                var prefs = await _userPreferenceRepository.GetAsync(userId) ?? new Data.Entity.UserPreference(userId);
                prefs.SavedSearchToken = (string)payload.token;
                await _userPreferenceRepository.UpsertAsync(prefs);
                return NoContent();
            }
            else
            {
                return BadRequest("User not authenticated");
            }
        }
    }
}
