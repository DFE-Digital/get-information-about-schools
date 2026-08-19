using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.Establishments
{
    public class EstablishmentWriteApiService : IEstablishmentWriteService
    {
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly HttpClientWrapper _httpClient;

        public EstablishmentWriteApiService(HttpClientWrapper httpClient, IEstablishmentReadService establishmentReadService)
        {
            _httpClient = httpClient;
            _establishmentReadService = establishmentReadService;
        }

        public async Task<ApiResponse<object, ValidationEnvelopeDto[]>> AddLinkedEstablishmentAsync(int parentUrn, int urnToLink, int linkTypeId, DateTime linkDate, IPrincipal principal)
        {
            var set = (await _establishmentReadService.GetLinkedEstablishmentsAsync(parentUrn, principal)).ToList();

            if (!set.Any(x => x.Urn == urnToLink))
            {
                set.Add(new LinkedEstablishmentModel
                {
                    LinkDate = linkDate,
                    LinkTypeId = linkTypeId,
                    Urn = urnToLink
                });
            }

            return await SaveLinkedEstablishmentsAsync(parentUrn, set.ToArray(), principal);
        }

        public async Task<ApiResponse<AmalgamateMergeResult, AmalgamateMergeValidationEnvelope[]>> AmalgamateOrMergeAsync(AmalgamateMergeRequest request, IPrincipal principal)
            => await _httpClient.PostAsync<AmalgamateMergeResult, AmalgamateMergeValidationEnvelope[]>("establishment/amalgamate-merge", request, principal);

        public async Task<ApiResponse<BulkCreateAcademiesProgress, NewAcademyRequestValidationEnvelope[]>> BulkCreateAcademies(NewAcademyRequest[] newAcademyRequests, IPrincipal principal)
            => await _httpClient.PostAsync<BulkCreateAcademiesProgress, NewAcademyRequestValidationEnvelope[]>("establishment/bulk-create-academies", newAcademyRequests, principal);


        public async Task<ApiResponse<BulkCreateFreeSchoolsResult>> BulkCreateFreeSchoolsAsync(string filename, IPrincipal principal)
            => (await _httpClient.PostMultipartAsync<BulkCreateFreeSchoolsResult>("establishment/bulk-create-free-schools", null, filename, principal));

        public async Task<ApiResponse<BulkCreateFreeSchoolsResult>> BulkCreateFreeSchoolsGetProgressAsync(Guid taskId, IPrincipal principal)
            => await _httpClient.GetAsync<BulkCreateFreeSchoolsResult>($"establishment/bulk-create-free-schools/progress/{taskId}", principal);


        public async Task<ApiResponse<BulkUpdateProgressModel>> BulkAssociateEstabs2GroupsAsync(string filename, IPrincipal principal)
            => await _httpClient.PostMultipartAsync<BulkUpdateProgressModel>("establishment/bulk-associate-to-groups", null, filename, principal);

        public async Task<ApiResponse<BulkUpdateProgressModel>> BulkAssociateEstabs2GroupsGetProgressAsync(Guid taskId, IPrincipal principal)
            => await _httpClient.GetAsync<BulkUpdateProgressModel>($"establishment/bulk-associate-to-groups/progress/{taskId}", principal);


        public async Task<ApiResponse<BulkUpdateProgressModel>> BulkUpdateAsync(BulkUpdateDto bulkUpdateInfo, IPrincipal principal)
        {
            var overrideCRFlag = bulkUpdateInfo.OverrideCRProcess ? "?overridecr=true" : string.Empty;
            return (await _httpClient.PostMultipartAsync<BulkUpdateProgressModel>("establishment/bulk-update" + overrideCRFlag,
                bulkUpdateInfo, bulkUpdateInfo.FileName, principal));
        }

        public async Task<BulkUpdateProgressModel> BulkUpdateAsync_GetProgressAsync(Guid taskId, IPrincipal principal)
        {
            return (await _httpClient.GetAsync<BulkUpdateProgressModel>($"bulk-update/progress/{taskId}", principal)).GetResponse();
        }

        public async Task<ApiResponse> ConfirmAsync(int urn, IPrincipal principal) => await _httpClient.PostAsync($"establishment/{urn}/confirm", null, principal);

        public async Task<ApiResponse> ConfirmGovernanceAsync(int urn, IPrincipal principal) => await _httpClient.PostAsync($"establishment/{urn}/governance/confirm", null, principal);

        public async Task<ApiResponse<int>> CreateNewAsync(EstablishmentModel model, bool generateEstablishmentNumber, IPrincipal principal)
        {
            return Unwrap(await _httpClient.PostAsync<ApiResultDto<int>>($"establishment?autogenestabno={generateEstablishmentNumber.ToString().ToLower()}", model, principal));
        }

        public async Task<ApiResponse<object, ValidationEnvelopeDto[]>> DeleteLinkedEstablishmentAsync(int urn, int linkId, IPrincipal principal)
        {
            var set = (await _establishmentReadService.GetLinkedEstablishmentsAsync(urn, principal)).ToList();
            set.Remove(set.First(x => x.Id == linkId));
            return await SaveLinkedEstablishmentsAsync(urn, set.ToArray(), principal);
        }

        public async Task<ApiResponse<BulkCreateAcademiesProgress>> GetBulkCreateAcademiesProgress(Guid id, IPrincipal principal)
            => await _httpClient.GetAsync<BulkCreateAcademiesProgress>($"establishment/bulk-create-academies/progress/{id}", principal);

        public async Task<ApiResponse> PartialUpdateAsync(EstablishmentModel model, EstablishmentFieldList fieldsToUpdate, IPrincipal principal)
        {
            Guard.IsNotNull(model.Urn, () => new Exception("Urn on the model parameter cannot be null for a partial update"));

            var propertiesToUpdate = ReflectionHelper.GetProperties(fieldsToUpdate)
                .Where(property => ReflectionHelper.GetPropertyValue<bool>(fieldsToUpdate, property))
                .Select(x => new { Key = x, Value = ReflectionHelper.GetPropertyValue(model, x) });

            var payload = propertiesToUpdate.ToDictionary(x => x.Key, x => x.Value);

            return await _httpClient.PatchAsync($"establishment/{model.Urn}", payload, principal);
        }

        public async Task SaveAsync(EstablishmentModel model, bool overrideCR, DateTime? effectiveDate, IPrincipal principal)
        {
            var parameters = new Dictionary<string, string>
            {
                [nameof(overrideCR).ToLower()] = (principal.IsInRole(EdubaseRoles.ROLE_BACKOFFICE) && overrideCR).ToString().ToLower(),
                [nameof(effectiveDate)] = effectiveDate?.ToString("yyyy-MM-dd")
            };
            var queryString = string.Join("&", parameters.Where(x => x.Value != null).Select(x => $"{x.Key}={x.Value}"));

            model.Location = null; // always set to null just in case the easting/northing values have been changed - apparently by setting this to null, it signifies to Texuna, they should re-calc the lat/long values.

            if (model.SENIds != null && !model.SENIds.Any()) model.SENIds = null; // they don't like an empty array.

            await _httpClient.PutAsync($"establishment?{queryString}", model, principal);
        }

        public async Task<ApiResponse<object, ValidationEnvelopeDto[]>> SaveLinkedEstablishmentsAsync(int urn, LinkedEstablishmentModel[] linkedEstablishmentModels, IPrincipal principal)
        {
            return await _httpClient.PutAsync<object, ValidationEnvelopeDto[]>($"establishment/{urn}/linked-establishments", linkedEstablishmentModels, principal);
        }

        public async Task<ApiResponse> UpdateGovernanceModeAsync(int urn, eGovernanceMode mode, IPrincipal principal)
            => await _httpClient.PutAsync($"establishment/{urn}/governancemode?governanceModeId={(int)mode}", null, principal);

        public async Task<ValidationEnvelopeDto> ValidateAsync(EstablishmentModel model, IPrincipal principal)
        {
            if (model.SENIds != null && !model.SENIds.Any()) model.SENIds = null; // they don't like an empty array.

            return (await _httpClient.PutAsync<ValidationEnvelopeDto>($"establishment/validate", model, principal)).Response;
        }

        public async Task<ApiResponse<NewAcademyRequestValidationEnvelope[]>> ValidateBulkCreateAcademies(NewAcademyRequest[] newAcademyRequests, IPrincipal principal)
            => await _httpClient.PostAsync<NewAcademyRequestValidationEnvelope[]>("establishment/bulk-create-academies/validate", newAcademyRequests, principal);

        public async Task<ValidationEnvelopeDto> ValidateCreateAsync(EstablishmentModel model, bool generateEstablishmentNumber, IPrincipal principal)
        {
            return (await _httpClient.PostAsync<ValidationEnvelopeDto>($"establishment/validate?autogenestabno={generateEstablishmentNumber.ToString().ToLower()}", model, principal)).Response;
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
    }
}
