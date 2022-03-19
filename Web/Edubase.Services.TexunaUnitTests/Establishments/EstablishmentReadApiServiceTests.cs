using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Establishments;
using Edubase.Services.Texuna;
using Moq;
using Xunit;

namespace Edubase.Services.TexunaUnitTests.Establishments
{
    public class EstablishmentReadApiServiceTests
    {
        private readonly Mock<IEstablishmentReadService> _mockEstablishmentReadService = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
        private readonly Mock<HttpClientWrapper> _mockHttpClientWrapper = new Mock<HttpClientWrapper>();

        public EstablishmentReadApiServiceTests()
        {

        }

        [Fact()]
        public async Task CanAccess_Returns_True()
        {
            //Arrange
           _mockHttpClientWrapper.Setup(x => x.GetAsync<bool>(It.IsAny<string>(), It.IsAny<IPrincipal>())).Returns(Task.FromResult(true));


           _mockEstablishmentReadService.Setup(e => e.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            //Act


            //Assert

            //await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.EditHelpdesk(4));
        }
    }
}
