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
            CloseDate = true;
            ReasonEstablishmentClosed = true;
            LAESTAB = true;
            HeadteacherDetails = true;
            GenderOfEntry = true;
            MainEmailAddress = IsUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = IsUserLoggedIn;
        }
    }
}