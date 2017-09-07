using System.Threading.Tasks;
using AutoMapper;
using Edubase.Services.Establishments;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Web.Resources;
using Edubase.Web.UI.Areas.Establishments.Controllers;
using Moq;
using NUnit.Framework;

namespace Edubase.UnitTest.Controllers
{
    [TestFixture]
    public class EstablishmentControllerTest : UnitTestBase<EstablishmentController>
    {
        private Mock<IEstablishmentReadService> establishmentReadServiceMock;
        private Mock<IGroupReadService> groupReadServiceMock;
        private Mock<IMapper> mapperMock;
        private Mock<IEstablishmentWriteService> establishmentWriteServiceMock;
        private Mock<ICachedLookupService> cachedLookupServiceMock;
        private Mock<IResourcesHelper> resourceHelperMock;
        private Mock<ISecurityService> securityServiceMock;

        public EstablishmentControllerTest()
        {
            ObjectUnderTest = new EstablishmentController(AddMock<IEstablishmentReadService>().Object,
                AddMock<IGroupReadService>().Object, 
                AddMock<IMapper>().Object,
                AddMock<IEstablishmentWriteService>().Object, 
                AddMock<ICachedLookupService>().Object,
                AddMock<IResourcesHelper>().Object, 
                AddMock<ISecurityService>().Object);

        }

        public async Task Estab_EditDetails_Null_Id()
        {
            
        }
    }
}
