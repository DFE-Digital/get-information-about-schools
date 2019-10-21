using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.Establishments;
using System.Threading.Tasks;
using System.Web.Http;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SearchApiController : ApiController
    {
        private readonly ITokenRepository _tokenRepository;

        public SearchApiController(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        [Route("api/tokenize"), HttpPost]
        public async Task<dynamic> Tokenize()
        {
            var formstate = await Request.Content.ReadAsStringAsync();
            var token = new Token(formstate);
            await _tokenRepository.CreateAsync(token);
            return new { token = token.Id };
        }

        
    }
}
