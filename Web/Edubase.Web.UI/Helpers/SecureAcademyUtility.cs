using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Security;

namespace Edubase.Web.UI.Helpers
{
    public static class SecureAcademyUtility
    {
        public static int[] GetAcademyOpeningsEstablishmentTypeByTypeGroupId(string establishmentCode) =>
            new[]
            {
                // 57 is the establishment type code for Secure 16-19 Academies.
                establishmentCode == null || !IsSecure16to19AcademyEstablishmentCode(establishmentCode)
                    ? (int) eLookupEstablishmentTypeGroup.Academies
                    : (int) eLookupEstablishmentTypeGroup.Secure16To19Academy
            };

        public static IEnumerable<EstablishmentLookupDto> FilterEstablishmentType(
            IEnumerable<EstablishmentLookupDto> establishmentTypes, string establishmentCode = null)
        {
            if (!string.IsNullOrWhiteSpace(establishmentCode) && int.TryParse(establishmentCode, out var estabCode))
                establishmentTypes = establishmentTypes.Where(et => et.Code == estabCode.ToString());

            return establishmentTypes;
        }

        public static string GetAcademyOpeningPageTitle(string establishmentCode)
        {
            return string.IsNullOrWhiteSpace(establishmentCode) ||
                   !IsSecure16to19AcademyEstablishmentCode(establishmentCode)
                ? "Manage academy openings"
                : "Manage 16-19 secure academy openings";
        }

        public static string GetEstablishmentCodeForSecure16To19Academy(IPrincipal user)
        {
            var loggedInUser = ((ClaimsIdentity) user.Identity).Claims.First(c => c.Value == "YCS").Value;

            if (!string.IsNullOrWhiteSpace(loggedInUser) &&
                loggedInUser.Equals(EdubaseRoles.YCS, StringComparison.OrdinalIgnoreCase))
                return "57";

            return null;
        }

        private static bool IsSecure16to19AcademyEstablishmentCode(string establishmentCode) =>
            establishmentCode.Trim().Equals("57", StringComparison.OrdinalIgnoreCase);
    }
}
