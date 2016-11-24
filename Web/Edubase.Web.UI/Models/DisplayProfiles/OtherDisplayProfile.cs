using Edubase.Services.Enums;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Models.DisplayProfiles
{
    public class OtherDisplayProfile : EstablishmentDisplayProfile
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