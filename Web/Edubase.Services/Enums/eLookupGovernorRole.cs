namespace Edubase.Services.Enums
{

    /// <summary>
    /// Enum values much match the <c>id</c> column of the StaffRole table.
    /// </summary>
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
        Group_SharedChairOfLocalGoverningBody = 10,
        Establishment_SharedChairOfLocalGoverningBody = 11,
        Group_SharedLocalGovernor = 12,
        Establishment_SharedLocalGovernor = 13,
        GovernanceProfessionalToALocalAuthorityMaintainedSchool = 15,
        GovernanceProfessionalToAFederation = 16,
        GovernanceProfessionalToAnIndividualAcademyOrFreeSchool = 17,
        GovernanceProfessionalToAMat = 18,
        Group_SharedGovernanceProfessional = 19,
        Establishment_SharedGovernanceProfessional = 20,
        Member_Individual = 21,
        Member_Organisation = 22,
        GovernanceProfessionalToASecureSat = 23,
        GovernanceProfessionalToASat = 24,
    }
}
