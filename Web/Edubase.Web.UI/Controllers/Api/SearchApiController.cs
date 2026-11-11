using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

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
        public async Task<object> Tokenize()
        {
            using var reader = new StreamReader(Request.Body);
            var formstate = await reader.ReadToEndAsync();
            var token = new Token(formstate);
            await _tokenRepository.CreateAsync(token);
            return new { token = token.Id };
        }
    }
}
