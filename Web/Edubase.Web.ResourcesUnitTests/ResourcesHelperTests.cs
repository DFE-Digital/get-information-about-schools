using System.Security.Principal;
using Edubase.Services.Enums;
using Moq;
using Xunit;

namespace Edubase.Web.Resources.UnitTests;

public class ResourcesHelperTests
{

    [Theory()]
    [InlineData(null, null)]
    [InlineData(null, false)]
    [InlineData(null, true)]
    [InlineData(eLookupEstablishmentTypeGroup.LAMaintainedSchools, null)]
    [InlineData(eLookupEstablishmentTypeGroup.LAMaintainedSchools, false)]
    [InlineData(eLookupEstablishmentTypeGroup.LAMaintainedSchools, true)]
    [InlineData(eLookupEstablishmentTypeGroup.Academies, null)]
    [InlineData(eLookupEstablishmentTypeGroup.Academies, false)]
    [InlineData(eLookupEstablishmentTypeGroup.Academies, true)]
    [InlineData(eLookupEstablishmentTypeGroup.OtherTypes, null)]
    [InlineData(eLookupEstablishmentTypeGroup.OtherTypes, false)]
    [InlineData(eLookupEstablishmentTypeGroup.OtherTypes, true)]
    public void GetResourceStringForEstablishmentTest(eLookupEstablishmentTypeGroup? establishmentType, bool? isAuthenticated)
    {
        //there is very little to test on this method, except that even when provided with junk it returns null
        //values have been provided to test the various routes the method may provide a result
        IPrincipal user = null;
        var resourcesHelper = new ResourcesHelper();

        if(isAuthenticated != null)
        {
            var mockUser = new Mock<IPrincipal>();
            mockUser.Setup(x => x.Identity.IsAuthenticated).Returns(isAuthenticated ?? false);
            user = mockUser.Object;
        }
        var result = resourcesHelper.GetResourceStringForEstablishment("something-ridiculous-that-should-never-be-found", establishmentType, user);

        Assert.Null(result);
    }
}
