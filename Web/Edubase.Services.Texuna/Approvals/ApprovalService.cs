using Edubase.Services.Approvals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Approvals.Models;
using Edubase.Services.Domain;
using System.Security.Principal;

namespace Edubase.Services.Texuna.Approvals
{
    public class ApprovalService : IApprovalService
    {
        private readonly HttpClientWrapper _httpClient;
   
        public ApprovalService(HttpClientWrapper httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse> ActionAsync(PendingChangeRequestAction payload, IPrincipal principal)
            =>  await _httpClient.PostAsync("approvals/pending", payload, principal);

        public async Task<int> CountAsync(IPrincipal principal)
        {
            var result = await _httpClient.GetAsync<PendingApprovalsResult>("approvals/pending?skip=0&take=0", principal);
            if (result.Success) return result.Response.Count;
            else throw new Exception("Unexpected errors returned from API");
        }

        public async Task<ApiResponse<PendingApprovalsResult>> GetAsync(int skip, int take, IPrincipal principal)
        {
            var result = await _httpClient.GetAsync<PendingApprovalsResult>($"approvals/pending?skip={skip}&take={take}", principal);
            if (result.Success) return result;
            else throw new Exception("Unexpected errors returned from API");
        }
    }
}
