using System.IO;
using System.Text;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    [ApiController]
    [Route("api/tokenize")]
    public class SearchApiController : ControllerBase
    {
        private readonly ITokenRepository _tokenRepository;

        public SearchApiController(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken] // Use this for API endpoints unless CSRF protection is explicitly required
        public async Task<IActionResult> Tokenize()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var formstate = await reader.ReadToEndAsync();

            var token = new Token(formstate);
            await _tokenRepository.CreateAsync(token);

            return Ok(new { token = token.Id });
        }
    }
}
