
namespace Edubase.Services.Enums
{

#if(TEXAPI)

    public enum eLookupEstablishmentTypeGroup
    {
        Academies = 2,
        ChildrensCentres = 4,
        Colleges = 1,
        FreeSchools = 3,
        IndependentSchools = 6,
        LAMaintainedSchools = 7,
        OtherTypes = 10,
        SpecialSchools = 8,
        Universities = 5,
        WelshSchools = 9,
    }

#else

    public enum eLookupEstablishmentTypeGroup
    {
        Colleges = 1,
		Universities = 2,
		IndependentSchools = 3,
		LAMaintainedSchools = 4,
		SpecialSchools = 5,
		WelshSchools = 6,
		OtherTypes = 7,
		Academies = 8,
		FreeSchools = 9,
		ChildrensCentres = 10,
    }

#endif

}
