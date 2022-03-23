using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.EditPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Lookup;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.Texuna.Models;
using Edubase.Services.TexunaUnitTests.FakeData;
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
            Assert.True(result.ReturnValue);
        }

        [Fact]
        public async Task CanAccess_Returns_False_For_Successful_Response_From_Api()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<BoolResult>(true) { Response = new BoolResult() { Value = false } };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<BoolResult>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.CanAccess(It.IsAny<int>(), It.IsAny<IPrincipal>());

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<BoolResult>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.False(result.ReturnValue);
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
            }
            };

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

        [Fact]
        public async Task GetChangeHistoryDownloadAsync_Returns_Correct_Filtered_Change_History_Items_For_Download()
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

            _mockHttpClientWrapper.Setup(x => x.PostAsync<FileDownloadDto>(It.IsAny<string>(), It.IsAny<EstablishmentChangeHistoryDownloadFilters>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetChangeHistoryDownloadAsync(123456, new EstablishmentChangeHistoryDownloadFilters() { ApprovedBy = "Approver1" }, user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.PostAsync<FileDownloadDto>(It.IsAny<string>(), It.IsAny<EstablishmentChangeHistoryDownloadFilters>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Name, result.Name);
        }

        [Fact]
        public async Task GetGovernanceChangeHistoryAsync_Returns_Correct_Number_Of_Change_History_Items()
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
            var result = await service.GetGovernanceChangeHistoryAsync(123456, 1, 1, "Urn", user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<ApiPagedResult<EstablishmentChangeDto>>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Items.Count, result.Items.Count);
        }

        [Fact]
        public async Task GetGovernanceChangeHistoryDownloadAsync_Returns_Correct_Change_History_Items_For_Download()
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
            var result = await service.GetGovernanceChangeHistoryDownloadAsync(123456, DownloadType.xlsx, user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<FileDownloadDto>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Name, result.Name);
        }

        [Fact]
        public async Task GetDisplayPolicyAsync_Returns_Correct_Establishment_Response()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<EstablishmentDisplayEditPolicy>(true)
            {
                Response = new EstablishmentDisplayEditPolicy()
                {
                    HeadteacherLabel = "HeadLabel1",
                    HeadEmailAddressLabel = "HeadEmailAddress1"
                }
            };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<EstablishmentDisplayEditPolicy>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetDisplayPolicyAsync(new EstablishmentModel() { EstablishmentTypeGroupId = 4 }, user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<EstablishmentDisplayEditPolicy>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.HeadEmailAddressLabel, result.HeadEmailAddressLabel);
        }

        [Fact]
        public async Task GetDownloadAsync_Returns_Correct_Download_Data()
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
            var result = await service.GetDownloadAsync(123456, DownloadType.xlsx, user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<FileDownloadDto>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Name, result.Name);
        }

        [Fact]
        public async Task GetEditPolicyAsync_Returns_Correct_Establishment_Policy_Envelope()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<EstablishmentEditPolicyEnvelope>(true)
            {
                Response = new EstablishmentEditPolicyEnvelope()
                {
                    ApprovalsPolicy = new EstablishmentApprovalsPolicy()
                    {
                        Name = new ApprovalPolicy() { ApproverName = "Approver1" }
                    },
                    EditPolicy = new EstablishmentDisplayEditPolicy() { }
                }
            };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<EstablishmentEditPolicyEnvelope>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetEditPolicyAsync(new EstablishmentModel() { Name = "EstablishmentName1" }, user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<EstablishmentEditPolicyEnvelope>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.ApprovalsPolicy.Name.ApproverName, result.ApprovalsPolicy.Name.ApproverName);
        }

        [Fact]
        public async Task GetEditPolicyByUrnAsync_Returns_Correct_Establishment_Policy_Envelope()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<EstablishmentEditPolicyEnvelope>(true)
            {
                Response = new EstablishmentEditPolicyEnvelope()
                {
                    ApprovalsPolicy = new EstablishmentApprovalsPolicy()
                    {
                        Name = new ApprovalPolicy() { ApproverName = "Approver1" }
                    }
                }
            };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<EstablishmentEditPolicyEnvelope>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetEditPolicyByUrnAsync(123456, user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<EstablishmentEditPolicyEnvelope>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.ApprovalsPolicy.Name.ApproverName, result.ApprovalsPolicy.Name.ApproverName);
        }

        [Fact]
        public async Task GetEstablishmentNameAsync_Calls_GetAsync()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<EstablishmentModel>(true) { Response = new EstablishmentModel() { Urn = 123456, Name = "Establishment1" } };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<EstablishmentModel>(It.IsAny<string>(), It.IsAny<IPrincipal>(), false))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            _ = await service.GetEstablishmentNameAsync(123456, user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<EstablishmentModel>(It.IsAny<string>(), It.IsAny<IPrincipal>(), false), Times.Once());
        }

        [Fact]
        public async Task GetLinkedEstablishmentsAsync_Returns_Correct_Number_Of_Linked_Establishments()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<List<LinkedEstablishmentModel>>(true)
            {
                Response = new List<LinkedEstablishmentModel>
                {
                    new LinkedEstablishmentModel()
                    {
                        Urn = 123456
                    },
                    new LinkedEstablishmentModel()
                    {
                        Urn = 123457
                    }
                }
            };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<List<LinkedEstablishmentModel>>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetLinkedEstablishmentsAsync(123456, user) as List<LinkedEstablishmentModel>;

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<List<LinkedEstablishmentModel>>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Count, result.Count);
        }

        [Fact]
        public async Task GetModelChangesAsync_Calls_GetAsync()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<EstablishmentModel>(true) { Response = new EstablishmentModel() { Urn = 123456, Name = "Establishment1", IEBTModel = new IEBTModel() { Proprietors = null } } };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<EstablishmentModel>(It.IsAny<string>(), It.IsAny<IPrincipal>(), false))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            _ = await service.GetModelChangesAsync(
                new EstablishmentModel()
            {
                Urn = 123457,
                IEBTModel = new IEBTModel() { Proprietors = null }
            }, new EstablishmentApprovalsPolicy()
            {
                Urn = new ApprovalPolicy()
                {
                    ApproverName = "Approver1",
                },
                IEBTDetail = new IEBTFieldList<ApprovalPolicy>() { Notes = new ApprovalPolicy() { } }
            }, user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<EstablishmentModel>(It.IsAny<string>(), It.IsAny<IPrincipal>(), false), Times.Once());
            _mockCachedLookupService.Setup(x => x.IsLookupField(It.IsAny<string>())).Returns(It.IsAny<bool>());
        }

        [Fact]
        public async Task GetModelChangesAsync_Calls_CachedLookup_Service()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<EstablishmentModel>(true) { Response = new EstablishmentModel() { Urn = 123456, Name = "Establishment1" } };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<EstablishmentModel>(It.IsAny<string>(), It.IsAny<IPrincipal>(), false))
                .ReturnsAsync(() => apiResponse);
            _mockCachedLookupService.Setup(x => x.IsLookupField(It.IsAny<string>()))
                .Returns(It.IsAny<bool>());

            //Act
            var service = GetEstablishmentReadApiService();
            _ = await service.GetModelChangesAsync(new EstablishmentModel()
            {
                Urn = 123456,
                IEBTModel =  new IEBTModel()
                {
                    Proprietors = new List<ProprietorModel>()
                    {
                        new ProprietorModel()
                        {
                            Id = 1, Name = "Name1", Address3 = "Address1",
                            CountyId = 1, Email = "Email1", Locality = "Locality1",
                            Postcode = "Postcode1", Street = "Street1", TelephoneNumber = "12345", Town = "Town1"
                        }
                    }
                }
            }, new EstablishmentModel()
            {
                Urn = 123456,
                IEBTModel = new IEBTModel()
                {
                    Proprietors = new List<ProprietorModel>()
                    {
                        new ProprietorModel()
                        {
                            Id = 2, Name = "Name2", Address3 = "Address2",
                            CountyId = 2, Email = "Email2", Locality = "Locality2",
                            Postcode = "Postcode2", Street = "Street2", TelephoneNumber = "12346", Town = "Town2"
                        },
                        new ProprietorModel()
                        {
                            Id = 3, Name = "Name3", Address3 = "Address3",
                            CountyId = 3, Email = "Email3", Locality = "Locality3",
                            Postcode = "Postcode3", Street = "Street3", TelephoneNumber = "12347", Town = "Town3"
                        }
                    }
                }
            }, new EstablishmentApprovalsPolicy()
            {
                Urn = new ApprovalPolicy()
                {
                    ApproverName = "Approver1",
                },
                IEBTDetail = new IEBTFieldList<ApprovalPolicy>() { Notes = new ApprovalPolicy() { } }
            });

            //Assert
            _mockCachedLookupService.Verify(x => x.IsLookupField(It.IsAny<string>()), Times.Exactly(9));
        }

        [Fact]
        public async Task GetPermissibleLocalGovernorsAsync_Returns_Correct_Number_Of_Lookup_Data()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<List<LookupDto>>(true)
            {
                Response = new List<LookupDto>()
                {
                    new LookupDto() { Id = 1, Name = "LookupName1"  },
                    new LookupDto() { Id = 2, Name = "LookupName2"  }
                }
            };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<List<LookupDto>>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetPermissibleLocalGovernorsAsync(123456, user) as List<LookupDto>;

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<List<LookupDto>>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Count, result.Count);
        }

        [Fact]
        public async Task GetPermittedStatusIdsAsync_Returns_Correct_Length_Lookup_Array()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<List<LookupDto>>(true)
            {
                Response = new List<LookupDto>()
                {
                    new LookupDto() { Id = 1, Name = "LookupName1"  },
                    new LookupDto() { Id = 2, Name = "LookupName2"  }
                }
            };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<List<LookupDto>>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.GetPermittedStatusIdsAsync(user) as int[];

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<List<LookupDto>>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Count, result.Length);
        }

        [Fact]
        public async Task SearchAsync_Returns_Correct_Number_Of_Search_Results()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<ApiPagedResult<EstablishmentSearchResultModel>>(true)
            {
                Response = new ApiPagedResult<EstablishmentSearchResultModel>()
                {
                    Items = new List<EstablishmentSearchResultModel>()
                    {
                        new EstablishmentSearchResultModel() { Urn = 12345 },
                        new EstablishmentSearchResultModel() { Urn = 12346 }
                    },
                    Count = 2,
                }
            };

            _mockHttpClientWrapper.Setup(x => x.PostAsync<ApiPagedResult<EstablishmentSearchResultModel>>(It.IsAny<string>(), It.IsAny<EstablishmentSearchPayload>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.SearchAsync(new EstablishmentSearchPayload() { Filters = new EstablishmentSearchFilters() { UKPRN = "50050" } }, user);

            //Assert
            _mockHttpClientWrapper.Verify(x => x.PostAsync<ApiPagedResult<EstablishmentSearchResultModel>>(It.IsAny<string>(), It.IsAny<EstablishmentSearchPayload>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Count, result.Count);
        }

        [Fact]
        public async Task SuggestAsync_Returns_Correct_Number_Of_Search_Results()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();
            var apiResponse = new ApiResponse<List<EstablishmentSuggestionItem>>(true)
            {
                Response = new List<EstablishmentSuggestionItem>()
                {
                   new EstablishmentSuggestionItem() { Urn = 123456 },
                   new EstablishmentSuggestionItem() { Urn = 123467 }
                }
            };

            _mockHttpClientWrapper.Setup(x => x.GetAsync<List<EstablishmentSuggestionItem>>(It.IsAny<string>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => apiResponse);

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.SuggestAsync("SearchText", user, 1) as List<EstablishmentSuggestionItem>;

            //Assert
            _mockHttpClientWrapper.Verify(x => x.GetAsync<List<EstablishmentSuggestionItem>>(It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            Assert.Equal(apiResponse.Response.Count, result.Count);
        }

        [Fact]
        public async Task SuggestAsync_Returns_Empty_Search_Results_For_Empty_Search_String_Text()
        {
            //Arrange
            var user = new UserClaimsPrincipalFake().GetUserClaimsPrincipal();

            //Act
            var service = GetEstablishmentReadApiService();
            var result = await service.SuggestAsync("", user, 1);

            //Assert
            Assert.Empty(result);
        }

        

        private EstablishmentReadApiService GetEstablishmentReadApiService()
        {
            return new EstablishmentReadApiService(_mockHttpClientWrapper.Object, _mockCachedLookupService.Object);
        }

    }
}
