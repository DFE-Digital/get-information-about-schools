using Edubase.Services.Enums;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Models.DisplayProfiles
{
    public class WelshDisplayProfile : EstablishmentDisplayProfile
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
            => typeGroup == eLookupEstablishmentTypeGroup.WelshSchools && type == eLookupEstablishmentType.WelshEstablishment;

        protected override void ConfigureInternal()
        {
            var isUserLoggedIn = Principal.Identity.IsAuthenticated;
            var isSchoolClosed = Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

            CloseDate = isSchoolClosed;
            ReasonEstablishmentClosed = isSchoolClosed;
            ReligiousCharacter = true;
            LastChangedDate = isUserLoggedIn;
            LAESTAB = true;
            Section41Approved = true;
        }
    }
}