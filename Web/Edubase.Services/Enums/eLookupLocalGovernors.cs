
namespace Edubase.Services.Enums
{
    public enum eLookupLocalGovernors
    {
        HasLocalGovernors = 1,
		HasLocalGovernorsAndSharedLocalGovernorsWithOtherAcademiesInTheTrust = 2,
		HasSharedLocalGovernorsWithOtherAcademiesInTheTrust = 3,
		ThisAcademyIsPartOfAMultiacademyTrustAndDoesNotHaveLocalGovernors = 4,
    }
}   
