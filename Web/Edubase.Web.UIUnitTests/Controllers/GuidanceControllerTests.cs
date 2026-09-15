using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Services;
using Edubase.Web.UI.Controllers;
using Edubase.Web.UI.Controllers.Api;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Guidance;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Controllers
{
    public class GuidanceControllerTests
    {
        private static GuidanceController CreateController(
            ISqlLaNameCodeRepository repository = null)
        {
            return new GuidanceController(
                Mock.Of<IBlobService>(),
                repository ?? Mock.Of<ISqlLaNameCodeRepository>());
        }

        private static ISqlLaNameCodeRepository RepositoryReturning(
            params SqlLaNameCode[] rows)
        {
            var mock = new Mock<ISqlLaNameCodeRepository>();
            mock.Setup(x => x.GetAllAsync()).ReturnsAsync(rows.ToList());
            return mock.Object;
        }

        private static SqlLaNameCode Row(string group, string name, string laCode, string gssCode)
            => new SqlLaNameCode { GroupCode = group, LaName = name, LaCode = laCode, GsLaCode = gssCode };

        [Fact]
        public async Task LaNameCodes_SplitsRowsIntoCorrectGroups_AndMapsFields()
        {
            var repository = RepositoryReturning(
                Row("english", "A place in England", "869", "A06000037"),
                Row("welsh", "A place in Wales", "681", "A06000015"),
                Row("other", "A place somewhere", "420", "A06000053"));

            var controller = CreateController(repository);

            var result = Assert.IsType<ViewResult> (await controller.LaNameCodes());
            var model = Assert.IsType<GuidanceLaNameCodeViewModel>(result.Model);

            var english = Assert.Single(model.EnglishLas);
            Assert.Equal("A place in England", english.LaName);
            Assert.Equal("869", english.LaCode);
            Assert.Equal("A06000037", english.OnsLaCode);

            var welsh = Assert.Single(model.WelshLas);
            Assert.Equal("A place in Wales", welsh.LaName);
            Assert.Equal("681", welsh.LaCode);
            Assert.Equal("A06000015", welsh.OnsLaCode);

            var other = Assert.Single(model.OtherLas);
            Assert.Equal("A place somewhere", other.LaName);
            Assert.Equal("420", other.LaCode);
            Assert.Equal("A06000053", other.OnsLaCode);
        }

        [Fact]
        public async Task LaNameCodes_GroupCodeMatchIsCaseSensitive()
        {
            var repository = RepositoryReturning(
                Row("English", "Wrong Case", "999", "E99999999"));

            var controller = CreateController(repository);

            var result = Assert.IsType<ViewResult> (await controller.LaNameCodes());
            var model = Assert.IsType<GuidanceLaNameCodeViewModel>(result.Model);

            Assert.Empty(model.EnglishLas);
        }

        [Fact]
        public async Task LaNameCodes_UnrecognisedGroupCode_IsExcluded()
        {
            var repository = RepositoryReturning(
                Row("english", "Valid", "301", "E06000001"),
                Row("danish", "Not Shown", "998", "S99999999"));

            var controller = CreateController(repository);

            var result = Assert.IsType<ViewResult> (await controller.LaNameCodes());
            var model = Assert.IsType<GuidanceLaNameCodeViewModel>(result.Model);

            Assert.Single(model.EnglishLas);
            Assert.Empty(model.WelshLas);
            Assert.Empty(model.OtherLas);
        }

        [Fact]
        public async Task LaNameCodes_EmptyResult_ReturnsEmptyLists()
        {
            var repository = RepositoryReturning();

            var controller = CreateController(repository);

            var result = Assert.IsType<ViewResult> (await controller.LaNameCodes());
            var model = Assert.IsType<GuidanceLaNameCodeViewModel>(result.Model);

            Assert.Empty(model.EnglishLas);
            Assert.Empty(model.WelshLas);
            Assert.Empty(model.OtherLas);
        }

        [Fact]
        public async Task LaNameCodes_CallsRepositoryGetAllAsync_ExactlyOnce()
        {
            var mock = new Mock<ISqlLaNameCodeRepository>();
            mock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SqlLaNameCode>());

            var controller = CreateController(mock.Object);

            await controller.LaNameCodes();
            mock.Verify(x => x.GetAllAsync(), Times.Once);
        }
    }
}
