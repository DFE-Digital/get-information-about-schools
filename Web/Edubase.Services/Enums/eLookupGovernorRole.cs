
namespace Edubase.Services.Enums
{

#if (TEXAPI)

    public enum eLookupGovernorRole
    {
        NA = 14,
        ChairOfGovernors = 1,
        ChairOfLocalGoverningBody = 8,
        Governor = 2,
        LocalGovernor = 9,
        ChairOfTrustees = 3,
        Trustee = 4,
        Member = 5,
        AccountingOfficer = 6,
        ChiefFinancialOfficer = 7,
        Establishment_SharedChairOfLocalGoverningBody = 10,
        Group_SharedChairOfLocalGoverningBody = 11,
        Establishment_SharedLocalGovernor = 12,
        Group_SharedLocalGovernor = 13,
    }

#else

    public enum eLookupGovernorRole
    {
        AccountingOfficer = 1,
		ChairOfGovernors = 2,
		ChairOfLocalGoverningBody = 3,
		ChairOfTrustees = 4,
		ChiefFinancialOfficer = 5,
		Governor = 6,
		LocalGovernor = 7,
		Member = 8,
		Trustee = 9,
    }

#endif

}
