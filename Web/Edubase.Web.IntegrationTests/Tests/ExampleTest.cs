using Edubase.Web.IntegrationTests.Helpers;

namespace Edubase.Web.IntegrationTests.Tests;


public class ExampleTest
{
    private readonly GiasWebApplicationFactory _webApplicationFactory;

    public ExampleTest(GiasWebApplicationFactory webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task Test1()
    {
        using var client = _webApplicationFactory.CreateClient();
        var response = await client.GetAsync("/");

        var document = await response.GetDocumentAsync();
        Assert.NotNull(document.QuerySelector("body"));
    }
}
