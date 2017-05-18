using Edubase.Services.Establishments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Establishments.Models;
using System.Security.Principal;
using System.IO;
using Edubase.Services.Domain;

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
            if (model.Urn.HasValue)
            {
                await _httpClient.PutAsync($"establishment", model, principal);
            }
            else
            {

            }

            // todo: texchange: patch and post
        }

        public async Task<BulkUpdateProgressModel> BulkUpdateAsync(BulkUpdateDto bulkUpdateInfo, IPrincipal principal)
        {
            return await _httpClient.PostMultipartAsync<BulkUpdateProgressModel>("establishment/bulk-update", bulkUpdateInfo, bulkUpdateInfo.FileName, principal);
        }

        public async Task<BulkUpdateProgressModel> BulkUpdateAsync_GetProgressAsync(Guid taskId, IPrincipal principal)
        {
            return await _httpClient.GetAsync<BulkUpdateProgressModel>($"bulk-update/progress/{taskId}", principal);
        }


    }
}
