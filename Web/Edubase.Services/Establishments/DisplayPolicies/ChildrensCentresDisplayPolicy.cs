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

            MainEmailAddress = IsUserLoggedIn;
            MainEmailAddressLabel = "Centre email";

            EstablishmentTypeLabel = "Provider type";
            WebsiteAddress = true;
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosed = IsSchoolClosed;
            CCOperationalHours = IsUserLoggedIn;
            CCGovernance = IsUserLoggedIn;
            CCGovernanceDetail = IsUserLoggedIn;
            CCLAContactDetail = IsUserLoggedIn;
            CCPhaseType = IsUserLoggedIn;
            CCDeliveryModel = IsUserLoggedIn;
            GroupCollaborationName = IsUserLoggedIn;
            CCGroupLeadCentre = IsUserLoggedIn;
            CCDisadvantagedArea = IsUserLoggedIn;
            CCDirectProvisionOfEarlyYears = IsUserLoggedIn;
            CCUnder5YearsOfAgeCount = IsUserLoggedIn;
        }
    }
}