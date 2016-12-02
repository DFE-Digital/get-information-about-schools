using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class WelshDisplayPolicy : EstablishmentDisplayPolicy
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