using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SearchApiController : ApiController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly ITokenRepository _tokenRepository;

        public SearchApiController(IEstablishmentReadService establishmentReadService, ITokenRepository tokenRepository)
        {
            _establishmentReadService = establishmentReadService;
            _tokenRepository = tokenRepository;
        }

        [Route("api/establishment-search"), HttpPost]
        public async Task<ApiPagedResult<EstablishmentSearchResultModel>> ProcessRequestAsync(EstablishmentSearchPayload payload) 
            => await _establishmentReadService.SearchAsync(payload, User);

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