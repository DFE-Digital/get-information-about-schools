using Edubase.Services.Domain;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Web.IntegrationTests.Helpers;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Request;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Response;
using Edubase.Web.UI.Areas.Governors.Models;
using Newtonsoft.Json;

namespace Edubase.Web.IntegrationTests.Tests.EditGovernors;

[Collection(IntegrationTestsCollectionMarker.Name)]
public sealed class EditGovernorTests 
{
    private readonly GiasWebApplicationFactory _webApplicationFactory;
    private readonly EdubaseApiServerFixture _apiServer;

    public EditGovernorTests(GiasWebApplicationFactory webApplicationFactory, EdubaseApiServerFixture apiServer)
    {
        _webApplicationFactory = webApplicationFactory;
        _apiServer = apiServer;
    }

    [Fact]
    public async Task Test()
    {
        // STUB APPOINTING BODIES
        HttpMappingRequest request = new(
            [
                new HttpMappingFile("governor-appointing-bodies", "test.json"),
                new HttpMappingFile("titles", "test2.json"),
                new HttpMappingFile("display-policy", "test3.json")
            ]);

        HttpMappedResponses mappedHttpResponses = await _apiServer.RegisterHttpMapping(request);

        List<LookupDto> mappedResponse =
            mappedHttpResponses.GetResponseById("governor-appointing-bodies")
                .GetResponseBody<List<LookupDto>>();

        List<LookupDto> mappedTitles =
            mappedHttpResponses.GetResponseById("titles")
                .GetResponseBody<List<LookupDto>>();

        GovernorDisplayPolicy displayPolicy =
            mappedHttpResponses.GetResponseById("display-policy")
                .GetResponseBody<GovernorDisplayPolicy>();

        using var client = _webApplicationFactory.CreateClient();

        StringContent content = new(
            JsonConvert.SerializeObject(
                new CreateEditGovernorViewModel()));

        var response = await client.PostAsync("/Establishment/Edit/100000/Governance/Edit/1", content);

        var document = await response.GetDocumentAsync();
        Assert.NotNull(document.QuerySelector("body"));
    }
}
