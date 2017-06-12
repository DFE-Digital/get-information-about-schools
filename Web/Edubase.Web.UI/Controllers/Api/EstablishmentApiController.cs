using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace Edubase.Web.UI.Controllers.Api
{
    public class EstablishmentApiController : ApiController
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        
        public EstablishmentApiController(IEstablishmentReadService establishmentReadService)
        {
            _establishmentReadService = establishmentReadService;
        }

        [Route("api/establishment/{urn:int}"), HttpGet]
        public async Task<ServiceResultDto<EstablishmentModel>> Get(int urn)
            => await _establishmentReadService.GetAsync(urn, User);
    }
}