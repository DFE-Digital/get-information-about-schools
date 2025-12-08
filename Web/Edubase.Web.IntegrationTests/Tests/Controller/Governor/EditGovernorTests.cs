using Edubase.Services.Domain;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Web.IntegrationTests.Helpers;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Request;
using Edubase.Web.IntegrationTests.WireMock.Mapping.Services.MappingService.Response;
using Edubase.Web.UI.Areas.Governors.Models;
using Newtonsoft.Json;

namespace Edubase.Web.IntegrationTests.Tests.Controller.Governor;

public sealed class EditGovernorTests
{
    /*
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

        var mappedHttpResponses = await _apiServer.RegisterHttpMapping(request);

        var mappedResponse =
            mappedHttpResponses.GetResponseById("governor-appointing-bodies")
                .GetResponseBody<List<LookupDto>>();

        var mappedTitles =
            mappedHttpResponses.GetResponseById("titles")
                .GetResponseBody<List<LookupDto>>();

        var displayPolicy =
            mappedHttpResponses.GetResponseById("display-policy")
                .GetResponseBody<GovernorDisplayPolicy>();

        using var client = _webApplicationFactory.CreateClient();

        StringContent content = new(
            JsonConvert.SerializeObject(
                new CreateEditGovernorViewModel()));

        var response = await client.PostAsync("/Establishment/Edit/100000/Governance/Edit/1", content);

        var document = await response.GetDocumentAsync();
        Assert.NotNull(document.QuerySelector("body"));
    }*/
}
