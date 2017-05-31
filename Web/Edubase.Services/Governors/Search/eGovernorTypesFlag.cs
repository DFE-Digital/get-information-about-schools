namespace Edubase.Services.Governors.Search
{
    public enum eGovernorTypesFlag
    {
        MultiAcademyTrusts, // filtered to governors associated with MATs
        AcademiesWithinMAT, // filtered to governors of academies associated with an MAT
        AcademiesWithinSAT, // filtered to governors of academies associated with an SAT
        GovsOfLAMaintained // filtered to governors of LA maintained type establishments
    }
}
