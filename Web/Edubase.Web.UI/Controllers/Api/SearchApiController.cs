using System.IO;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tokenize()
        {
            var formstate = await new StreamReader(Request.Body).ReadToEndAsync();
            var token = new Token(formstate);
            await _tokenRepository.CreateAsync(token);
            return Ok(new { token = token.Id });
        }
    }
}
