using Edubase.Data.DbContext;
using Edubase.Services.Establishments;
using Edubase.Web.UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using System.Security.Principal;
using Edubase.Services.Security;
using Edubase.Services.Establishments.Models;
using System.Security.Claims;
using Edubase.Services.Security.Permissions;
using Edubase.Common;

namespace Edubase.IntegrationTest.Services.Establishments
{
    [TestFixture]
    public class EstablishmentWriteServiceTest
    {
        [Test]
        public async Task MyTestMethod()
        {
            IocConfig.Register();
            using (var scope = IocConfig.Container.BeginLifetimeScope())
            {
                var id = new GenericIdentity("testadminuser");
                id.AddClaim(new Claim(EduClaimTypes.EditEstablishment,
                    new EditEstablishmentPermissions
                    {
                        Urns = new[] { 100053 }
                    }.ToClaimToken()));
                var principal = new ClaimsPrincipal(id);
                

                var read = scope.Resolve<IEstablishmentReadService>();
                var svc = scope.Resolve<IEstablishmentWriteService>();

                var model = (await read.GetAsync(100053, principal)).GetResult();

                model.AdditionalAddresses = new List<AdditionalAddressModel>();
                model.AdditionalAddresses.Add(new AdditionalAddressModel
                {
                    Line1 = "test street",
                    CityOrTown = "testington1"
                });
                await svc.SaveAsync(model, principal);


                var testModel = (await read.GetAsync(100053, principal)).GetResult();
                Assert.AreEqual("testington1", testModel.AdditionalAddresses[0].CityOrTown);

            }

        }
    }
}
