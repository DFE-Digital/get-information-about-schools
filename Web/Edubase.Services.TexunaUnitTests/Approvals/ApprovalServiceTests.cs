using Xunit;
using System.Threading.Tasks;
using Moq;
using Edubase.Services.Approvals.Models;
using System.Security.Principal;
using Edubase.Services.Domain;

namespace Edubase.Services.Texuna.Approvals.Tests;

public class ApprovalServiceTests
{
    private readonly Mock<IHttpClientWrapper> _mockHttpClientWrapper;
    private readonly Mock<IPrincipal> _mockPrincipal;
    private readonly ApprovalService _approvalService;

    public ApprovalServiceTests()
    {
        _mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
        _mockPrincipal = new Mock<IPrincipal>();
        _approvalService = new ApprovalService(_mockHttpClientWrapper.Object);
    }

    [Fact()]
    public void ApprovalService_ConstructorTest()
    {
        Assert.NotNull(_approvalService);
    }

    [Fact()]
    public async Task ActionAsyncTest()
    {
        var payload = new PendingChangeRequestAction()
        {
            Action = ePendingChangeRequestAction.Approve,
            ActionSpecifier = "test-action-specifier",
            Ids = [1, 2, 3],
            RejectionReason = null
        };

        var uri = "approvals/pending";

        var expectedResponse = new ApiResponse(true);

        _mockHttpClientWrapper.Setup(x => x.PostAsync(uri, payload, _mockPrincipal.Object)).ReturnsAsync(expectedResponse).Verifiable();

        var actualResponse = await _approvalService.ActionAsync(payload, _mockPrincipal.Object);

        _mockHttpClientWrapper.Verify();
        Assert.Equal(expectedResponse, actualResponse);
    }

    [Fact()]
    public async Task CountAsyncTest()
    {
        var uri = "approvals/pending?skip=0&take=0";
        var expectedResponseObject = new ApiResponse<PendingApprovalsResult>(true)
        {
            Response = new PendingApprovalsResult()
            {
                Count = 1,
                Items = new PendingApprovalItem[] { new PendingApprovalItem() { Id = 1 } }
            }
        };
        _mockHttpClientWrapper.Setup(x => x.GetAsync<PendingApprovalsResult>(uri, _mockPrincipal.Object))
            .ReturnsAsync(expectedResponseObject).Verifiable();

        var actualRepsonse = await _approvalService.CountAsync(_mockPrincipal.Object);

        _mockHttpClientWrapper.Verify();
        Assert.Equal(expectedResponseObject.Response.Count, actualRepsonse);
    }

    [Fact()]
    public async Task GetAsyncTest()
    {
        int skip = 12;
        int take = 24;
        string sortBy = "test-field";

        var expectedResponseObject = new ApiResponse<PendingApprovalsResult>(true)
        {
            Response = new PendingApprovalsResult()
            {
                Count = 1,
                Items = new PendingApprovalItem[] { new PendingApprovalItem() { Id = 1 } }
            }
        };

        var uri = $"approvals/pending?skip={skip}&take={take}&sortby={sortBy}";

        _mockHttpClientWrapper.Setup(x => x.GetAsync<PendingApprovalsResult>(uri, _mockPrincipal.Object))
            .ReturnsAsync(expectedResponseObject).Verifiable();

        var actualResponse = await _approvalService.GetAsync(skip, take, sortBy, _mockPrincipal.Object);

        _mockHttpClientWrapper.Verify();
        Assert.Equal<PendingApprovalsResult>(expectedResponseObject.Response, actualResponse);
    }
}
