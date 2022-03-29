using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;

namespace Edubase.Services
{
    public interface IClientWrapperBase
    {
        Task<HttpRequestMessage> CreateHttpRequestMessageAsync(HttpMethod method, string uri, IPrincipal principal, object requestBodyData = null);
        Task DeleteAsync(string uri, object data, IPrincipal principal);
        Task<ApiResponse<TResponse>> GetAsync<TResponse>(string uri, IPrincipal principal);
        Task<ApiResponse<TResponse>> GetAsync<TResponse>(string uri, IPrincipal principal, bool throwOnNotFound);
        Task<ApiResponse> PatchAsync(string uri, object data, IPrincipal principal);
        Task<ApiResponse> PostAsync(string uri, object data, IPrincipal principal);
        Task<ApiResponse<T>> PostAsync<T>(string uri, object data, IPrincipal principal);
        Task<ApiResponse<TSuccess, TValidationEnvelope>> PostAsync<TSuccess, TValidationEnvelope>(string uri, object payload, IPrincipal principal) where TValidationEnvelope : class;
        Task<ApiResponse<T>> PostMultipartAsync<T>(string uri, object data, string fileName, IPrincipal principal);
        Task<ApiResponse> PutAsync(string uri, object data, IPrincipal principal);
        Task<ApiResponse<T>> PutAsync<T>(string uri, object data, IPrincipal principal);
        Task<ApiResponse<TSuccess, TValidationEnvelope>> PutAsync<TSuccess, TValidationEnvelope>(string uri, object payload, IPrincipal principal) where TValidationEnvelope : class;
        string ToJsonIndented(HttpHeaders headers);
    }
}
