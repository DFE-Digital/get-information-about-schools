namespace Edubase.Web.IntegrationTests.WireMock.Options;
public enum WireMockServerMode
{
    LocalProcess, // WireMock.NET StandAlone
    LocalContainer, // TODO TestContainers - and ContainerOptions (either .NET or Java)
    Remote, // RemotelyStarted server (either .NET or Java)
}
