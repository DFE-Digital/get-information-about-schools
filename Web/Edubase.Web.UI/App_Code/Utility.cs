using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Domain;
using Edubase.Services.Enums;

namespace Edubase.Web.UI
{
    public static class Utility
    {
        public static int[] GetAcademyOpeningsEstablishmentTypeByTypeGroupId(string establishmentCode) =>
            new[]
            {
                // 57 is the establishment type code for Secure 16-19 Academies.
                establishmentCode == null || establishmentCode != "57"
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
    }
}
