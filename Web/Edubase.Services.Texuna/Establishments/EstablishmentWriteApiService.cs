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
using Edubase.Common;

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
            
        }

        /// <summary>
        /// Creates a new establishment and returns its URN
        /// </summary>
        /// <param name="model"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public async Task<int> CreateNewAsync(NewEstablishmentModel model, IPrincipal principal)
        {
            var apiModel = new EstablishmentModel();
            apiModel.Name = model.Name;
            apiModel.EstablishmentNumber = model.EstablishmentNumber.ToInteger();
            apiModel.EducationPhaseId = model.EducationPhaseId;
            apiModel.TypeId = model.EstablishmentTypeId;
            apiModel.LocalAuthorityId = model.LocalAuthorityId;
            apiModel.CCLAContactDetail = new ChildrensCentreLocalAuthorityDto();
            apiModel.IEBTModel = new IEBTModel();
            return (await _httpClient.PostAsync<ApiResultDto<int>>($"establishment?autogenestabno={model.GenerateEstabNumber.ToString().ToLower()}", apiModel, principal)).Value;
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
