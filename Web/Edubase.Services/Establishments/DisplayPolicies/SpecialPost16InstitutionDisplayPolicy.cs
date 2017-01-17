using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class SpecialPost16InstitutionDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
            => typeGroup == eLookupEstablishmentTypeGroup.OtherTypes && type == eLookupEstablishmentType.SpecialPost16Institution;

        protected override void ConfigureInternal()
        {
            CloseDate = true;
            ReasonEstablishmentClosedId = true;
            EstablishmentNumber = true;
            HeadteacherDetails = true;
            ProvisionBoardingId = true;
            Capacity = true;
            AgeRange = true;
            GenderId = true;
            Contact_WebsiteAddress = true;
            Section41ApprovedId = true;

        }
    }
}