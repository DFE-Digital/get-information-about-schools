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
            ReasonEstablishmentClosedId = IsSchoolClosed;
            ReligiousCharacterId = true;
            LastChangedDate = IsUserLoggedIn;
            EstablishmentNumber = true;
            Section41ApprovedId = true;
        }
    }
}