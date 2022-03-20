using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Lookup;
using Edubase.Services.Texuna;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.Texuna.Models;
using Edubase.Services.TexunaUnitTests.FakeData;
using Edubase.Services.TexunaUnitTests.Mocks;
using Edubase.Web.UI;
using Moq;
using Xunit;

namespace Edubase.Services.TexunaUnitTests.Establishments
{
    public class EstablishmentReadApiServiceTests
    {
        private readonly Mock<IHttpClientWrapper> _mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
        private readonly Mock<ICachedLookupService> _mockCachedLookupService = new Mock<ICachedLookupService>();

        public EstablishmentReadApiServiceTests()
        {

        }

        [Fact]
        public async Task CanAccess_Returns_True()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<BoolResult>(true) { Response = new BoolResult() { Value = true } };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<BoolResult>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);
            
            //Act
            var service = new EstablishmentReadApiService(_mockHttpClientWrapper.Object, _mockCachedLookupService.Object);

            var result = service.CanAccess(100000, user);


            //Assert

        }

        [Fact]
        public async Task CanEditAsync_Returns_True()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<BoolResult>(true) { Response = new BoolResult() { Value = true } };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<BoolResult>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = new EstablishmentReadApiService(_mockHttpClientWrapper.Object, _mockCachedLookupService.Object);

            var result = service.CanAccess(100000, user);


            //Assert

        }

        //private EstablishmentReadApiService GetEstablishmentReadApiService()
        //{
        //    return new EstablishmentReadApiService(_mockHttpClientWrapper.Object, _mockCachedLookupService.Object);
        //}

        //private HttpClientWrapper CreateWrapper(MockHttpMessageHandler mockHandler) => new HttpClientWrapper(new HttpClient(mockHandler), IocConfig.CreateJsonMediaTypeFormatter(), new Mock<IClientStorage>(MockBehavior.Loose).Object);

    }
}
