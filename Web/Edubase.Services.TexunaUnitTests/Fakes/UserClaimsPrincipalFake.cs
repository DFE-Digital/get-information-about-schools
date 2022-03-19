using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.TexunaUnitTests.FakeData
{
    public class UserClaimsPrincipalFake
    {
        public ClaimsPrincipal GetUserClaimsPrincipal()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, "discoverytest842@gmail.com"),
                //new Claim(ClaimTypes.NameIdentifier, "1"),
                //new Claim("custom-claim", "example claim value"),
                //new Claim(CustomClaimTypes.UserId, "00000000-0000-0000-0000-000000000000"),
                //new Claim(CustomClaimTypes.SessionId, "8cd6f4a2-85d7-4001-b53c-d7bb40b0ab67"),
                //new Claim(ClaimTypes.Email,"test@yahoo.com"),
                //new Claim(ClaimTypes.GivenName,"Abc"),
                //new Claim(ClaimTypes.Surname,"xyz"),
                //new Claim(CustomClaimTypes.OrganisationId, "123"),
                //new Claim(CustomClaimTypes.OrganisationName, "Test Org"),
                //new Claim(CustomClaimTypes.OrganisationCategoryId,"001"),
                //new Claim(CustomClaimTypes.OrganisationEstablishmentTypeId, "00"),
                //new Claim(CustomClaimTypes.OrganisationHighAge, "13"),
                //new Claim(CustomClaimTypes.OrganisationLowAge, "2"),
                //new Claim(CustomClaimTypes.EstablishmentNumber,"89"),
                //new Claim(CustomClaimTypes.LocalAuthorityNumber,"98"),
                //new Claim(CustomClaimTypes.UniqueReferenceNumber,"121"),
                //new Claim(CustomClaimTypes.UniqueIdentifier,"007"),
                //new Claim(CustomClaimTypes.UKProviderReferenceNumber, "23432")
            }, "mock"));

            return user;
        }

        //public ClaimsPrincipal GetLAApproverClaimsPrincipal()
        //{
        //    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.Email, "discoverytest842+LAApprover@gmail.com"),
        //        new Claim(ClaimTypes.NameIdentifier, "1"),
        //        new Claim(CustomClaimTypes.UserId, "00000000-0000-0000-0000-000000000000"),
        //        new Claim(CustomClaimTypes.SessionId, "8cd6f4a2-85d7-4001-b53c-d7bb40b0ab67"),
        //        new Claim(ClaimTypes.Email,"test@yahoo.com"),
        //        new Claim(ClaimTypes.GivenName,"Abc"),
        //        new Claim(ClaimTypes.Surname,"xyz"),
        //        new Claim(CustomClaimTypes.OrganisationId, "123"),
        //        new Claim(CustomClaimTypes.OrganisationName, "Test Org"),
        //        new Claim(CustomClaimTypes.OrganisationCategoryId,"002"),
        //        new Claim(CustomClaimTypes.OrganisationHighAge, "20"),
        //        new Claim(CustomClaimTypes.OrganisationLowAge, "2"),
        //        new Claim(CustomClaimTypes.EstablishmentNumber,"89"),
        //        new Claim(CustomClaimTypes.LocalAuthorityNumber,"98"),
        //        new Claim(CustomClaimTypes.UniqueReferenceNumber,"121"),
        //        new Claim(CustomClaimTypes.UniqueIdentifier,"007"),
        //        new Claim(CustomClaimTypes.UKProviderReferenceNumber, "23432"),
        //        new Claim(ClaimTypes.Role,"GIAPApprover")
        //    }, "mock"));

        //    return user;
        //}

        //public ClaimsPrincipal GetSATApproverClaimsPrincipal()
        //{
        //    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.Email, "discoverytest842+SATApprover@gmail.com"),
        //        new Claim(ClaimTypes.NameIdentifier, "1"),
        //        new Claim("custom-claim", "example claim value"),
        //        new Claim(CustomClaimTypes.UserId, "00000000-0000-0000-0000-000000000000"),
        //        new Claim(CustomClaimTypes.SessionId, "8cd6f4a2-85d7-4001-b53c-d7bb40b0ab67"),
        //        new Claim(ClaimTypes.Email,"test@yahoo.com"),
        //        new Claim(ClaimTypes.GivenName,"Abc"),
        //        new Claim(ClaimTypes.Surname,"xyz"),
        //        new Claim(CustomClaimTypes.OrganisationId, "123"),
        //        new Claim(CustomClaimTypes.OrganisationName, "Test Org"),
        //        new Claim(CustomClaimTypes.OrganisationCategoryId,"013"),
        //        new Claim(CustomClaimTypes.OrganisationHighAge, "20"),
        //        new Claim(CustomClaimTypes.OrganisationLowAge, "2"),
        //        new Claim(CustomClaimTypes.EstablishmentNumber,"89"),
        //        new Claim(CustomClaimTypes.LocalAuthorityNumber,"98"),
        //        new Claim(CustomClaimTypes.UniqueReferenceNumber,"121"),
        //        new Claim(CustomClaimTypes.UniqueIdentifier,"007"),
        //        new Claim(CustomClaimTypes.UKProviderReferenceNumber, "23432"),
        //        new Claim(ClaimTypes.Role,"GIAPApprover")
        //    }, "mock"));

        //    return user;
        //}

        //public ClaimsPrincipal GetAdminUserClaimsPrincipal()
        //{
        //    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.Email,"discoverytest842+ADMIN@gmail.com"),
        //        new Claim(ClaimTypes.NameIdentifier, "1"),
        //        new Claim("custom-claim", "example claim value"),
        //        new Claim(CustomClaimTypes.UserId, "00000000-0000-0000-0000-000000000000"),
        //        new Claim(CustomClaimTypes.SessionId, "8cd6f4a2-85d7-4001-b53c-d7bb40b0ab67"),
        //        new Claim(ClaimTypes.Email,"test@yahoo.com"),
        //        new Claim(ClaimTypes.GivenName,"Abc"),
        //        new Claim(ClaimTypes.Surname,"xyz"),
        //        new Claim(CustomClaimTypes.OrganisationId, "123"),
        //        new Claim(CustomClaimTypes.OrganisationName, "Department for Education"),
        //        new Claim(CustomClaimTypes.OrganisationCategoryId,"789"),
        //        new Claim(CustomClaimTypes.OrganisationHighAge, "20"),
        //        new Claim(CustomClaimTypes.OrganisationLowAge, "2"),
        //        new Claim(CustomClaimTypes.EstablishmentNumber,"89"),
        //        new Claim(CustomClaimTypes.LocalAuthorityNumber,"98"),
        //        new Claim(CustomClaimTypes.UniqueReferenceNumber,"121"),
        //        new Claim(CustomClaimTypes.UniqueIdentifier,"007"),
        //        new Claim(CustomClaimTypes.UKProviderReferenceNumber, "23432"),
        //        new Claim(ClaimTypes.Role,"GIAPAdmin")
        //    }, "mock"));

        //    return user;
        //}

        //public ClaimsPrincipal GetFEApproverClaimsPrincipal()
        //{
        //    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.Email,"discoverytest842+FEApprover@gmail.com"),
        //        new Claim(ClaimTypes.NameIdentifier, "1"),
        //        new Claim(CustomClaimTypes.UserId, "00000000-0000-0000-0000-000000000000"),
        //        new Claim(CustomClaimTypes.SessionId, "8cd6f4a2-85d7-4001-b53c-d7bb40b0ab67"),
        //        new Claim(ClaimTypes.Email,"test@yahoo.com"),
        //        new Claim(ClaimTypes.GivenName,"Abc"),
        //        new Claim(ClaimTypes.Surname,"xyz"),
        //        new Claim(CustomClaimTypes.OrganisationId, "123"),
        //        new Claim(CustomClaimTypes.OrganisationName, "Test Org"),
        //        new Claim(CustomClaimTypes.OrganisationCategoryId, "001"),
        //        new Claim(CustomClaimTypes.OrganisationEstablishmentTypeId, "18"),
        //        new Claim(CustomClaimTypes.OrganisationHighAge, "20"),
        //        new Claim(CustomClaimTypes.OrganisationLowAge, "2"),
        //        new Claim(CustomClaimTypes.EstablishmentNumber,"89"),
        //        new Claim(CustomClaimTypes.LocalAuthorityNumber,"98"),
        //        new Claim(CustomClaimTypes.UniqueReferenceNumber,"121"),
        //        new Claim(CustomClaimTypes.UniqueIdentifier,"007"),
        //        new Claim(CustomClaimTypes.UKProviderReferenceNumber, "23432"),
        //        new Claim(ClaimTypes.Role,"GIAPApprover")
        //    }, "mock"));

        //    return user;
        //}

        //public ClaimsPrincipal GetLAUserClaimsPrincipal()
        //{
        //    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.Email,"discoverytest842+LAApprover@gmail.com"),
        //        new Claim(ClaimTypes.NameIdentifier, "1"),
        //        new Claim(CustomClaimTypes.UserId, "00000000-0000-0000-0000-000000000000"),
        //        new Claim(CustomClaimTypes.SessionId, "8cd6f4a2-85d7-4001-b53c-d7bb40b0ab67"),
        //        new Claim(ClaimTypes.Email,"test@yahoo.com"),
        //        new Claim(ClaimTypes.GivenName,"Abc"),
        //        new Claim(ClaimTypes.Surname,"xyz"),
        //        new Claim(CustomClaimTypes.OrganisationId, "123"),
        //        new Claim(CustomClaimTypes.OrganisationName, "Test Org"),
        //        new Claim(CustomClaimTypes.OrganisationCategoryId, "002"),
        //        new Claim(CustomClaimTypes.OrganisationEstablishmentTypeId, "18"),
        //        new Claim(CustomClaimTypes.OrganisationHighAge, "20"),
        //        new Claim(CustomClaimTypes.OrganisationLowAge, "2"),
        //        new Claim(CustomClaimTypes.EstablishmentNumber,"89"),
        //        new Claim(CustomClaimTypes.LocalAuthorityNumber,"98"),
        //        new Claim(CustomClaimTypes.UniqueReferenceNumber,"121"),
        //        new Claim(CustomClaimTypes.UniqueIdentifier,"007"),
        //        new Claim(CustomClaimTypes.UKProviderReferenceNumber, "23432"),
        //        new Claim(ClaimTypes.Role,"GIAPApprover")
        //    }, "mock"));

        //    return user;
        //}

        //public ClaimsPrincipal GetSpecificUserClaimsPrincipal(
        //    string organisationCategoryId,
        //    string organisationEstablishmentType,
        //    string role,
        //    int organisationLowAge,
        //    int organisationHighAge,
        //    string email = "testy.mctester@somewhere.net")
        //{
        //    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.Email, email),
        //        new Claim(ClaimTypes.NameIdentifier, "1"),
        //        new Claim(CustomClaimTypes.UserId, "00000000-0000-0000-0000-000000000000"),
        //        new Claim(CustomClaimTypes.SessionId, "8cd6f4a2-85d7-4001-b53c-d7bb40b0ab67"),
        //        new Claim(ClaimTypes.Email,"test@yahoo.com"),
        //        new Claim(ClaimTypes.GivenName,"Abc"),
        //        new Claim(ClaimTypes.Surname,"xyz"),
        //        new Claim(CustomClaimTypes.OrganisationId, "123"),
        //        new Claim(CustomClaimTypes.OrganisationName, "Test Org"),
        //        new Claim(CustomClaimTypes.OrganisationCategoryId, organisationCategoryId),
        //        new Claim(CustomClaimTypes.OrganisationEstablishmentTypeId, organisationEstablishmentType),
        //        new Claim(CustomClaimTypes.OrganisationHighAge, organisationHighAge.ToString()),
        //        new Claim(CustomClaimTypes.OrganisationLowAge, organisationLowAge.ToString()),
        //        new Claim(CustomClaimTypes.EstablishmentNumber,"89"),
        //        new Claim(CustomClaimTypes.LocalAuthorityNumber,"98"),
        //        new Claim(CustomClaimTypes.UniqueReferenceNumber,"121"),
        //        new Claim(CustomClaimTypes.UniqueIdentifier,"007"),
        //        new Claim(CustomClaimTypes.UKProviderReferenceNumber, "23432"),
        //        new Claim(ClaimTypes.Role, role)
        //    }, "mock"));

        //    return user;
        //}
    }
}
