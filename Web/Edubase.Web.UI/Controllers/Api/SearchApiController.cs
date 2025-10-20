using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SearchApiController : ControllerBase
    {
        private readonly ITokenRepository _tokenRepository;

        public SearchApiController(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        [Route("api/tokenize"), HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<dynamic> Tokenize()
        {
            var formstate = await Request.Content.ReadAsStringAsync();
            var token = new Token(formstate);
            await _tokenRepository.CreateAsync(token);
            return new { token = token.Id };
        }
    }
}
