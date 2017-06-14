using Edubase.Services.Establishments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Establishments.Models;
using System.Security.Principal;
using Edubase.Services.Domain;
using Edubase.Common;
using Edubase.Services.Security;
using Edubase.Services.Enums;
using Edubase.Common.Reflection;

namespace Edubase.Services.Texuna.Establishments
{
    public class EstablishmentWriteApiService : IEstablishmentWriteService
    {
        private readonly HttpClientWrapper _httpClient;
        
        public EstablishmentWriteApiService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SaveAsync(EstablishmentModel model, bool overrideCR, DateTime? effectiveDate, IPrincipal principal)
        {
            if (!model.HelpdeskPreviousLocalAuthorityId.HasValue)
                model.HelpdeskPreviousLocalAuthorityId = 189; // marcel: for the timebeing lets just hack it and put it in tech debt to resolve. At this stage it feels like we have more pressing matters

            var parameters = new Dictionary<string, string>
            {
                [nameof(overrideCR).ToLower()] = (principal.IsInRole(EdubaseRoles.ROLE_BACKOFFICE) && overrideCR).ToString().ToLower(),
                [nameof(effectiveDate)] = effectiveDate?.ToString("yyyy-MM-dd")
            };
            var queryString = string.Join("&", parameters.Where(x => x.Value != null).Select(x => $"{x.Key}={x.Value}"));
            await _httpClient.PutAsync($"establishment?{queryString}", model, principal);
        }

        public async Task<ValidationEnvelopeDto> ValidateAsync(EstablishmentModel model, IPrincipal principal)
        {
            if (!model.HelpdeskPreviousLocalAuthorityId.HasValue)
                model.HelpdeskPreviousLocalAuthorityId = 189; // marcel: for the timebeing lets just hack it and put it in tech debt to resolve. At this stage it feels like we have more pressing matters

            return (await _httpClient.PutAsync<ValidationEnvelopeDto>($"establishment/validate", model, principal)).Response;
        }
                
        /// <summary>
        /// Creates a new establishment and returns its URN
        /// </summary>
        /// <param name="model"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public async Task<ApiResponse<int>> CreateNewAsync(NewEstablishmentModel model, IPrincipal principal)
        {
            var apiModel = new EstablishmentModel
            {
                Name = model.Name,
                EstablishmentNumber = model.EstablishmentNumber.ToInteger(),
                EducationPhaseId = model.EducationPhaseId,
                TypeId = model.EstablishmentTypeId,
                LocalAuthorityId = model.LocalAuthorityId,
                CCLAContactDetail = new ChildrensCentreLocalAuthorityDto(),
                IEBTModel = new IEBTModel(),
                StatusId = (int) eLookupEstablishmentStatus.ProposedToOpen
            };

            return Unwrap(await _httpClient.PostAsync<ApiResultDto<int>>($"establishment?autogenestabno={model.GenerateEstabNumber.ToString().ToLower()}", apiModel, principal));
        }

        public async Task<BulkUpdateProgressModel> BulkUpdateAsync(BulkUpdateDto bulkUpdateInfo, IPrincipal principal)
        {
            return (await _httpClient.PostMultipartAsync<BulkUpdateProgressModel>("establishment/bulk-update", bulkUpdateInfo, bulkUpdateInfo.FileName, principal)).GetResponse();
        }

        public async Task<BulkUpdateProgressModel> BulkUpdateAsync_GetProgressAsync(Guid taskId, IPrincipal principal)
        {
            return (await _httpClient.GetAsync<BulkUpdateProgressModel>($"bulk-update/progress/{taskId}", principal)).GetResponse();
        }

        public async Task<ApiResponse> PartialUpdateAsync(EstablishmentModel model, EstablishmentFieldList fieldsToUpdate, IPrincipal principal)
        {
            Guard.IsNotNull(model.Urn, () => new Exception("Urn on the model parameter cannot be null for a partial update"));

            var propertiesToUpdate = ReflectionHelper.GetProperties(fieldsToUpdate)
                .Where(property => ReflectionHelper.GetPropertyValue<bool>(fieldsToUpdate, property))
                .Select(x => new {Key = x, Value = ReflectionHelper.GetPropertyValue(model, x)});

            var payload = propertiesToUpdate.ToDictionary(x => x.Key, x => x.Value);

            return await _httpClient.PatchAsync($"establishment/{model.Urn}", payload, principal);
        }

        private ApiResponse<T> Unwrap<T>(ApiResponse<ApiResultDto<T>> response)
        {
            return new ApiResponse<T>
            {
                Success = response.Success,
                Errors = response.Errors,
                Response = response.Response != null ? response.Response.Value : default(T)
            };
        }

        public async Task<ApiResponse<AmalgamateMergeResult, AmalgamateMergeValidationEnvelope[]>> AmalgamateOrMergeAsync(AmalgamateMergeRequest request, IPrincipal principal)
            => await _httpClient.PostAsync<AmalgamateMergeResult, AmalgamateMergeValidationEnvelope[]>("establishment/amalgamate-merge", request, principal);

        public async Task<ApiResponse<BulkCreateAcademiesProgress, NewAcademyRequestValidationEnvelope[]>> BulkCreateAcademies(NewAcademyRequest[] newAcademyRequests, IPrincipal principal)
            => await _httpClient.PostAsync<BulkCreateAcademiesProgress, NewAcademyRequestValidationEnvelope[]>("establishment/bulk-create-academies", newAcademyRequests, principal);

        public async Task<ApiResponse<NewAcademyRequestValidationEnvelope[]>> ValidateBulkCreateAcademies(NewAcademyRequest[] newAcademyRequests, IPrincipal principal)
            => await _httpClient.PostAsync<NewAcademyRequestValidationEnvelope[]>("establishment/bulk-create-academies/validate", newAcademyRequests, principal);

        public async Task<ApiResponse<BulkCreateAcademiesProgress>> GetBulkCreateAcademiesProgress(Guid id, IPrincipal principal)
            => await _httpClient.GetAsync<BulkCreateAcademiesProgress>($"establishment/bulk-create-academies/progress/{id}", principal);

    }
}
