
namespace Edubase.Services.Enums
{

#if (TEXAPI)

    public enum eLookupGovernorAppointingBody
    {
        NA = 16,
        AppointedByGbboard = 1,
        AppointedByAcademyMembers = 2,
        AppointedByFoundationtrust = 3,
        ElectedBySchoolStaff = 4,
        ElectedByParents = 5,
        ParentAppointedByGbboardDueToNoElectionCandidates = 6,
        NominatedByLAAndAppointedByGB = 7,
        NominatedByOtherBodyAndAppointedByGB = 8,
        ExofficioByVirtueOfOfficeAsHeadteacherprincipal = 9,
        ExofficioFoundationGovernorAppointedByFoundationByVirtueOfTheOfficeTheyHold = 10,
        OriginalSignatoryMembers = 11,
        FoundationsponsorMembers = 12,
        PersonsWhoAreAppointedByTheFoundationBodyOrSponsorIfApplicable = 13,
        AnyAdditionalMembersAppointedByTheMembersOfTheAcademyTrust = 14,
        InterimExecutiveBoard = 15,
    }

#else

    public enum eLookupGovernorAppointingBody
    {
        NA = 1,
		AppointedByGbboard = 2,
		AppointedByAcademyMembers = 3,
		AppointedByFoundationtrust = 4,
		ElectedBySchoolStaff = 5,
		ElectedByParents = 6,
		ParentAppointedByGbboardDueToNoElectionCandidates = 7,
		NominatedByLAAndAppointedByGB = 8,
		NominatedByOtherBodyAndAppointedByGB = 9,
		ExofficioByVirtueOfOfficeAsHeadteacherprincipal = 10,
		ExofficioFoundationGovernorAppointedByFoundationByVirtueOfTheOfficeTheyHold = 11,
		OriginalSignatoryMembers = 12,
		FoundationsponsorMembers = 13,
		PersonsWhoAreAppointedByTheFoundationBodyOrSponsorIfApplicable = 14,
		AnyAdditionalMembersAppointedByTheMembersOfTheAcademyTrust = 15,
    }

#endif
}
