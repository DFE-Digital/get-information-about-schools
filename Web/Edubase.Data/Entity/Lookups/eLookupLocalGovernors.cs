
namespace Edubase.Data.Entity.Lookups
{
    public enum eLookupLocalGovernors
    {
        HasLocalGovernors = 1,
		HasLocalGovernorsAndSharedLocalGovernorsWithOtherAcademiesInTheTrust = 2,
		HasSharedLocalGovernorsWithOtherAcademiesInTheTrust = 3,
		ThisAcademyIsPartOfAMultiacademyTrustAndDoesNotHaveLocalGovernors = 4,
    }
}   
