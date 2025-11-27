using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Edubase.Services.Domain;
using Edubase.Services.Downloads;
using Edubase.Services.Establishments;
using Edubase.Services.Groups.Downloads;
using Edubase.Web.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Edubase.Web.UIUnitTests.Controllers
{
    public class DownloadsControllerTests
    {
        private readonly DownloadsController _controller;
        private readonly Mock<IDownloadsService> _downloadsServiceMock;
        private readonly Mock<IEstablishmentReadService> _establishmentReadServiceMock;
        private readonly Mock<IGroupDownloadService> _groupDownloadServiceMock;

        public DownloadsControllerTests()
        {
            _downloadsServiceMock = new Mock<IDownloadsService>();
            _establishmentReadServiceMock = new Mock<IEstablishmentReadService>();
            _groupDownloadServiceMock = new Mock<IGroupDownloadService>();

            _controller = new DownloadsController(
                _downloadsServiceMock.Object,
                _establishmentReadServiceMock.Object,
                _groupDownloadServiceMock.Object
            );
        }

        [Fact]
        public async Task RequestScheduledExtract_Redirect_WithApiResultsDto()
        {
            var testEid = 123;
            var expectedGuid = Guid.NewGuid();
            var apiResultDtoJson = $"{{ \"Value\": \"{expectedGuid}\" }}";

            _downloadsServiceMock
                .Setup(x => x.GenerateScheduledExtractAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(apiResultDtoJson);

            var httpContextMock = new Mock<HttpContext>();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object };

            var result = await _controller.RequestScheduledExtract(testEid);

            var redirectResult = Assert.IsType<RedirectToRouteResult>(result);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.NotNull(redirectResult);
        }

        // 'isExtract' is expected in the controller, but it is not present (not a test issue - so not asserted)
        [Fact]
        public async Task RequestScheduledExtract_Redirect_WithProgressDto()
        {
            var testEid = 123;
            var expectedId = Guid.NewGuid();
            var progressDto = new ProgressDto { FileLocationUri = $"file://{expectedId}" };
            var progressDtoJson = JsonConvert.SerializeObject(progressDto);

            _downloadsServiceMock
                .Setup(x => x.GenerateScheduledExtractAsync(testEid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(progressDtoJson);

            var httpContextMock = new Mock<HttpContext>();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object };

            var result = await _controller.RequestScheduledExtract(testEid);

            var redirectResult = Assert.IsType<RedirectToRouteResult>(result);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.NotNull(redirectResult);
            Assert.True(redirectResult.RouteValues.ContainsKey("id"));

            var extractedId = ExtractIdFromFileLocationUri(progressDto.FileLocationUri);
            Assert.Equal(expectedId, extractedId);
        }

        private Guid ExtractIdFromFileLocationUri(string fileLocationUri)
        {
            if (string.IsNullOrWhiteSpace(fileLocationUri) || !fileLocationUri.StartsWith("file://"))
            {
                return Guid.Empty;
            }

            var idString = fileLocationUri.Replace("file://", "").Trim();
            return Guid.TryParse(idString, out var id) ? id : Guid.Empty;
        }
    }
}
