using System.Security.Claims;
using System.Threading.Tasks;
using Edubase.Services.Security;
using Edubase.Web.UI.App_Start;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Edubase.Web.UIUnitTests.App_Start
{
    public class AuthenticationServiceRegistrationTests
    {
        [Theory]
        [InlineData("EdubasePolicy", "OrdinaryUser", true)]
        [InlineData("CanManageAcademyOpeningsOrSecureAcademy16To19", EdubaseRoles.EFADO, false)]
        [InlineData("CanManageAcademyOpeningsOrSecureAcademy16To19", EdubaseRoles.YCS, false)]
        [InlineData("CanBulkAssociateEstabs2Groups", AuthorizedRoles.CanBulkAssociateEstabs2Groups, false)]
        [InlineData("CanBulkCreateFreeSchools", AuthorizedRoles.CanBulkCreateFreeSchools, false)]
        [InlineData("CanBulkUpdateEstablishments", AuthorizedRoles.CanBulkUpdateEstablishments, false)]
        public async Task AuthorizationPolicy_RequiresSignIn_AndPreservesRoleRestrictions(string policyName, string roles, bool allowsOtherRoles)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddApplicationAuthentication(configuration);
            var role = roles.Split(',')[0];

            using var provider = services.BuildServiceProvider();
            var authorization = provider.GetRequiredService<IAuthorizationService>();
            var permittedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, role) }, "Test"));
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, role) }));
            var otherUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "UnrelatedRole") }, "Test"));

            Assert.True((await authorization.AuthorizeAsync(permittedUser, null, policyName)).Succeeded);
            Assert.False((await authorization.AuthorizeAsync(anonymousUser, null, policyName)).Succeeded);
            Assert.Equal(allowsOtherRoles, (await authorization.AuthorizeAsync(otherUser, null, policyName)).Succeeded);
        }
    }
}
