using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using System.Threading.Tasks;
using System.Web.Http;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SearchApiController : ApiController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        
        public SearchApiController(IEstablishmentReadService establishmentReadService)
        {
            _establishmentReadService = establishmentReadService;
        }

        [Route("api/establishment-search"), HttpPost]
        public async Task<ApiPagedResult<EstablishmentSearchResultModel>> ProcessRequestAsync(EstablishmentSearchPayload payload) 
            => await _establishmentReadService.SearchAsync(payload, User);
    }
}