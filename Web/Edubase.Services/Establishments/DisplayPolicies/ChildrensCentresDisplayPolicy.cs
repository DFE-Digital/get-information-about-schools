using Edubase.Common;
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class ChildrensCentresDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
        {
            return typeGroup.Equals(eLookupEstablishmentTypeGroup.ChildrensCentres)
            && type.Equals(eLookupEstablishmentType.ChildrensCentre);
        }

        protected override void ConfigureInternal()
        {
            HeadteacherDetails = true;
            HeadteacherLabel = "Manager";
            HeadEmailAddressLabel = "Manager email";
            HeadEmailAddress = IsUserLoggedIn;

            Contact_EmailAddress = IsUserLoggedIn;
            MainEmailAddressLabel = "Centre email";

            EstablishmentTypeLabel = "Provider type";
            Contact_WebsiteAddress = true;
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosedId = IsSchoolClosed;
            CCOperationalHoursId = IsUserLoggedIn;
            CCGovernanceId = IsUserLoggedIn;
            CCGovernanceDetail = IsUserLoggedIn;
            CCLAContactDetail = IsUserLoggedIn;
            CCPhaseTypeId = IsUserLoggedIn;
            CCDeliveryModelId = IsUserLoggedIn;
            GroupCollaborationName = IsUserLoggedIn;
            CCGroupLeadId = IsUserLoggedIn;
            CCDisadvantagedAreaId = IsUserLoggedIn;
            CCDirectProvisionOfEarlyYearsId = IsUserLoggedIn;
            CCUnder5YearsOfAgeCount = IsUserLoggedIn;
        }
    }
}