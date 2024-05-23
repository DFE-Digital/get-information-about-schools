using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SearchApiController : ApiController
    {
        private readonly ITokenRepository _tokenRepository;
        private IEnumerable<string> tokenHeaderValues;

        public SearchApiController(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        [System.Web.Http.Route("api/tokenize"), System.Web.Http.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<dynamic> Tokenize()
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

            var formstate = await Request.Content.ReadAsStringAsync();
            var token = new Token(formstate);
            await _tokenRepository.CreateAsync(token);
            return new { token = token.Id };
        }
    }
}
