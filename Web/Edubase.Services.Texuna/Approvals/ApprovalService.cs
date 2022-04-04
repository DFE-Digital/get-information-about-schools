using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Approvals;
using Edubase.Services.Approvals.Models;
using Edubase.Services.Domain;

namespace Edubase.Services.Texuna.Approvals
{
    public class ApprovalService : IApprovalService
    {
        private readonly IHttpClientWrapper _httpClient;

        public ApprovalService(IHttpClientWrapper httpClient) => _httpClient = httpClient;

        public async Task<ApiResponse> ActionAsync(PendingChangeRequestAction payload, IPrincipal principal) =>
            await _httpClient.PostAsync("approvals/pending", payload, principal);

        public async Task<int> CountAsync(IPrincipal principal) =>
            (await _httpClient.GetAsync<PendingApprovalsResult>("approvals/pending?skip=0&take=0", principal))
            .GetResponse().Count;

        public async Task<PendingApprovalsResult> GetAsync(int skip, int take, string sortBy, IPrincipal principal) =>
            (await _httpClient.GetAsync<PendingApprovalsResult>($"approvals/pending?skip={skip}&take={take}&sortby={sortBy}", principal))
            .GetResponse();
    }
}
