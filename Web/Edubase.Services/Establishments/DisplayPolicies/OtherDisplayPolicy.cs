using Edubase.Common;
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class OtherDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
        {
            return typeGroup.OneOfThese(eLookupEstablishmentTypeGroup.OtherTypes, eLookupEstablishmentTypeGroup.Colleges)
            &&
            type.OneOfThese(eLookupEstablishmentType.InstitutionFundedByOtherGovernmentDepartment,
                            eLookupEstablishmentType.OffshoreSchools,
                            eLookupEstablishmentType.SecureUnits,
                            eLookupEstablishmentType.ServiceChildrensEducation,
                            eLookupEstablishmentType.SixthFormCentres);
        }

        protected override void ConfigureInternal()
        {
            var isUserLoggedIn = Principal.Identity.IsAuthenticated;
            var isSchoolClosed = Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

            CloseDate = true;
            ReasonEstablishmentClosed = true;
            LAESTAB = true;
            HeadteacherDetails = true;
            GenderOfEntry = true;
            MainEmailAddress = isUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = isUserLoggedIn;
        }
    }
}