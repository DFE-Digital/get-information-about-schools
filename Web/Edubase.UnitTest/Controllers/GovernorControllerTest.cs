using System.Threading.Tasks;
using Edubase.Services.Establishments;
using Edubase.Services.Governors;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Services.Nomenclature;
using Edubase.Web.UI.Areas.Governors.Controllers;
using Edubase.Web.UI.Exceptions;
using Edubase.Web.UI.Helpers;
using NUnit.Framework;
using Shouldly;

namespace Edubase.UnitTest.Controllers
{
    [TestFixture]
    public class GovernorControllerTest : UnitTestBase<GovernorController>
    {
        [Test]
        public async Task Gov_Edit_Null_Params()
        {
            await ObjectUnderTest.Edit(null, null, null, null).ShouldThrowAsync(typeof(InvalidParameterException));
        }

        [SetUp]
        public void SetUpTest() => SetupObjectUnderTest();

        [TearDown]
        public void TearDownTest() => ResetMocks();

        [OneTimeSetUp]
        protected override void InitialiseMocks()
        {
            AddMock<IGovernorsReadService>();
            AddMock<NomenclatureService>();
            AddMock<ICachedLookupService>();
            AddMock<IGovernorsWriteService>();
            AddMock<IGroupReadService>();
            AddMock<IEstablishmentReadService>();
            AddMock<ILayoutHelper>();
            base.InitialiseMocks();
        }
    }
}
