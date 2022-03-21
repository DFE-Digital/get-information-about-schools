using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
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
        public async Task CanAccess_Returns_True_For_Successful_Response_From_Api()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<BoolResult>(true) { Response = new BoolResult() { Value = true } };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<BoolResult>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);
            
            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>());

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<BoolResult>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.True(result.Success);
        }

        [Fact]
        public async Task CanAccess_Throws_Exception_For_Unsuccessful_Response_From_Api()
        {
            //Arrange
            var apiResponse = new ApiResponse<BoolResult>(false) { Response = new BoolResult() { Value = false } };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<BoolResult>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();

            //Assert
            await Assert.ThrowsAsync<Exception>(async () => await service.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>()));
            _mockHttpClientWrapper.Verify(x => x.GetAsync<BoolResult>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
        }

        [Fact]
        public async Task CanEdit_Returns_True_For_Successful_Response_From_Api()
        {
            //Arrange
            var apiResponse = new ApiResponse<BoolResult>(true) { Response = new BoolResult() { Value = true } };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<BoolResult>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.CanEditAsync(It.IsAny<int>(), It.IsAny<IPrincipal>());
    
            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<BoolResult>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.True(result);
        }

        [Fact]
        public async Task GetAddressesByPostCodeAsync_Returns_Correct_Number_of_Addresses()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<AddressBaseResult[]>(true)
            {
                Response = new AddressBaseResult[]
            {
                new AddressBaseResult { BuildingNumber = "Building1", PostTown = "PostTown1", UPRN = "12345" },
                new AddressBaseResult { BuildingNumber = "Building2", PostTown = "PostTown2", UPRN = "12346" },
            }};

            _mockHttpClientWrapper.Setup(x => x.GetAsync<AddressBaseResult[]>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetAddressesByPostCodeAsync("PostCode", user) as List<AddressLookupResult>;

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<AddressBaseResult[]>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Length, result.Count);
        }

        [Fact]
        public async Task GetAddressesByPostCodeAsync_Returns_Empty_Address_List_On_Exception()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();

            _mockHttpClientWrapper.Setup(x => x.GetAsync<AddressBaseResult[]>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                 .Throws(new Exception());

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetAddressesByPostCodeAsync("PostCode", user);

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAsync_Returns_Correct_Establishment_Model()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<EstablishmentModel>(true) { Response = new EstablishmentModel() { Urn = 123456, Name = "Establishment1" } };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<EstablishmentModel>(It.IsAny<string>(), It.IsAny<IPrincipal>(), false))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetAsync(123456, user);

            var establishment = result.ReturnValue;
            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<EstablishmentModel>(It.IsAny<string>(), It.IsAny<IPrincipal>(), false), Times.Once());
            Assert.Equal(apiResponse.Response.Urn, establishment.Urn);
        }

        [Fact]
        public async Task GetChangeHistoryAsync_Returns_Correct_Number_Of_Change_History_Items()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<ApiPagedResult<EstablishmentChangeDto>>(true)
            {
                Response = new ApiPagedResult<EstablishmentChangeDto>()
                {
                    Items = new List<EstablishmentChangeDto>()
                    {
                        new EstablishmentChangeDto() { Urn = 12345, Id = "Id1" },
                        new EstablishmentChangeDto() { Urn = 12346, Id = "Id2" },
                    },
                    Count = 2,
                }
            };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<ApiPagedResult<EstablishmentChangeDto>>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetChangeHistoryAsync(123456, 1, 1, "Urn", user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<ApiPagedResult<EstablishmentChangeDto>>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Items.Count, result.Items.Count);
        }

        [Fact]
        public async Task GetChangeHistoryAsync_Returns_Correct_Number_Of_Filtered_Change_History_Items()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<ApiPagedResult<EstablishmentChangeDto>>(true)
            {
                Response = new ApiPagedResult<EstablishmentChangeDto>()
                {
                    Items = new List<EstablishmentChangeDto>()
                    {
                        new EstablishmentChangeDto() { Urn = 12345, Id = "Id1" },
                        new EstablishmentChangeDto() { Urn = 12346, Id = "Id2" },
                    },
                    Count = 2,
                }
            };

            _mockHttpClientWrapper.Setup(x => x.PostAsync<ApiPagedResult<EstablishmentChangeDto>>(It.IsAny<string>(), It.IsAny<EstablishmentChangeHistoryFilters>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetChangeHistoryAsync(123456, 1, 1, "Urn", new EstablishmentChangeHistoryFilters() { ApprovedBy = "Approver1" }, user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.PostAsync<ApiPagedResult<EstablishmentChangeDto>>(It.IsAny<string>(), It.IsAny<EstablishmentChangeHistoryFilters>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Items.Count, result.Items.Count);
        }

        [Fact]
        public async Task GetChangeHistoryDownloadAsync_Returns_Correct_Change_History_Items_For_Download()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<FileDownloadDto>(true)
            {
                Response = new FileDownloadDto()
                {
                    Name = "Name1",
                    Description = "Description1"
                }            
            };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<FileDownloadDto>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetChangeHistoryDownloadAsync(123456, DownloadType.xlsx, user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<FileDownloadDto>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Name, result.Name);
        }




        private EstablishmentReadApiService GetEstablishmentReadApiService()
        {
            return new EstablishmentReadApiService(_mockHttpClientWrapper.Object, _mockCachedLookupService.Object);
        }

    }
}
