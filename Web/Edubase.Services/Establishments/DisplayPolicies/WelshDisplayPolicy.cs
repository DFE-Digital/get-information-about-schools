using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class WelshDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
            => typeGroup == eLookupEstablishmentTypeGroup.WelshSchools && type == eLookupEstablishmentType.WelshEstablishment;

        protected override void ConfigureInternal()
        {
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosed = IsSchoolClosed;
            ReligiousCharacter = true;
            LastChangedDate = IsUserLoggedIn;
            LAESTAB = true;
            Section41Approved = true;
        }
    }
}