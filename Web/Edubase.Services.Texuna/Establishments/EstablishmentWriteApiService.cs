using Edubase.Services.Establishments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Establishments.Models;
using System.Security.Principal;

namespace Edubase.Services.Texuna.Establishments
{
    public class EstablishmentWriteApiService : IEstablishmentWriteService
    {
        private readonly HttpClientWrapper _httpClient;
        
        public EstablishmentWriteApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SaveAsync(EstablishmentModel model, IPrincipal principal)
        {
            throw new NotImplementedException("Awaiting the Create/Edit establishment APIs");

            if (model.Urn.HasValue)
            {
                await _httpClient.PutAsync($"establishment/{model.Urn}", model, principal);
            }
            else
            {

            }
        }
    }
}
