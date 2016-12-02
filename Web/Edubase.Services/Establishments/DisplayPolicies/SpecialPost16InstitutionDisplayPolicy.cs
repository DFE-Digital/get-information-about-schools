using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class SpecialPost16InstitutionDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
            => typeGroup == eLookupEstablishmentTypeGroup.OtherTypes && type == eLookupEstablishmentType.SpecialPost16Institution;

        protected override void ConfigureInternal()
        {
            var isUserLoggedIn = Principal.Identity.IsAuthenticated;
            var isSchoolClosed = Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

            CloseDate = true;
            ReasonEstablishmentClosed = true;
            LAESTAB = true;
            HeadteacherDetails = true;
            BoardingProvision = true;
            Capacity = true;
            AgeRange = true;
            GenderOfEntry = true;
            WebsiteAddress = true;
            Section41Approved = true;

        }
    }
}