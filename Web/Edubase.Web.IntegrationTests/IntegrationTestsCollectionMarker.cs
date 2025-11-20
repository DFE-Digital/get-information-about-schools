namespace Edubase.Web.IntegrationTests;
[CollectionDefinition(Name)]
public sealed class IntegrationTestsCollectionMarker : ICollectionFixture<EdubaseApiServerFixture>, IClassFixture<GiasWebApplicationFactory>
{
    public const string Name = "IntegrationTestsCollection";
}
