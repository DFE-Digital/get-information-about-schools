using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.EditPolicies;
using Xunit;
using ET = Edubase.Services.Enums.eLookupEstablishmentType;

namespace Edubase.ServicesUnitTests.Establishments.EditPolicies
{
    public class LookupOptionsFilterTests
    {
        [Fact]
        public void Academy16_19Secure_FilterEstablishmentStatuses()
        {
            var lookups = new List<LookupDto>() {
                new LookupDto { Id = (int) eLookupEstablishmentStatus.PendingApproval},
                new LookupDto { Id = (int) eLookupEstablishmentStatus.Open },
                new LookupDto { Id = (int) eLookupEstablishmentStatus.Quarantine }
            };

            var result = lookups.FilterForEstablishmentType((int) ET.AcademySecure16to19);

            Assert.Equal(2, result.Count());
            Assert.Equal((int) eLookupEstablishmentStatus.PendingApproval, result.ToArray()[0].Id);
            Assert.Equal((int) eLookupEstablishmentStatus.Open, result.ToArray()[1].Id);
        }

        [Fact]
        public void EstablishmentOtherType_UseAllEstablishmentStatuses()
        {
            var lookups = new List<LookupDto>() {
                new LookupDto { Id = (int) eLookupEstablishmentStatus.PendingApproval},
                new LookupDto { Id = (int) eLookupEstablishmentStatus.Open },
                new LookupDto { Id = (int) eLookupEstablishmentStatus.Quarantine }
            };

            var result = lookups.FilterForEstablishmentType((int) ET.SecureUnits);

            Assert.Equal(3, result.Count());
            Assert.Equal((int) eLookupEstablishmentStatus.PendingApproval, result.ToArray()[0].Id);
            Assert.Equal((int) eLookupEstablishmentStatus.Open, result.ToArray()[1].Id);
            Assert.Equal((int) eLookupEstablishmentStatus.Quarantine, result.ToArray()[2].Id);
        }
    }
}
