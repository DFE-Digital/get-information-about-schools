using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Lookup;
using Edubase.Services.Texuna;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.TexunaUnitTests.FakeData;
using Edubase.Services.TexunaUnitTests.Mocks;
using Edubase.Web.UI;
using Moq;
using Xunit;

namespace Edubase.Services.TexunaUnitTests.Establishments
{
    public class EstablishmentReadApiServiceTests
    {
        //private readonly Mock<IEstablishmentReadService> _mockEstablishmentReadService = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
       // private readonly Mock<HttpClientWrapper> _mockHttpClientWrapper = new Mock<HttpClientWrapper>();
        private readonly Mock<ICachedLookupService> _mockCachedLookupService = new Mock<ICachedLookupService>();

        public EstablishmentReadApiServiceTests()
        {

        }

        [Fact]
        public async Task CanAccess_Returns_True()
        {
            //Arrange
            var mockHandler = new MockHttpMessageHandler();
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
           //_mockHttpClientWrapper.Setup(x => x.GetAsync<bool>(It.IsAny<string>(), It.IsAny<IPrincipal>())).Returns(Task.FromResult(new ApiResponse<bool>(true)));


            //_mockEstablishmentReadService.Setup(e => e.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>()))
            //     .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            //Act
            var service = GetEstablishmentReadApiService(mockHandler);

            var result = await service.CanAccess(100000, user);

            //Assert

            //await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.EditHelpdesk(4));
        }

        private EstablishmentReadApiService GetEstablishmentReadApiService(MockHttpMessageHandler mockHandler)
        {
            return new EstablishmentReadApiService(CreateWrapper(mockHandler), _mockCachedLookupService.Object);
        }

        private HttpClientWrapper CreateWrapper(MockHttpMessageHandler mockHandler) => new HttpClientWrapper(new HttpClient(mockHandler), IocConfig.CreateJsonMediaTypeFormatter(), new Mock<IClientStorage>(MockBehavior.Loose).Object);

    }
}
