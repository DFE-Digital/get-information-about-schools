
namespace Edubase.Services.Enums
{
#if (TEXAPI)
    public enum eLookupGroupType
    {
        ChildrensCentresCollaboration = 9,
        ChildrensCentresGroup = 8,
        Federation = 1,
        MultiacademyTrust = 6,
        SchoolSponsor = 5,
        SingleacademyTrust = 10,
        Trust = 2,
        UmbrellaTrust = 7,
    }
#else
    public enum eLookupGroupType
    {
        ChildrensCentresCollaboration = 1,
		ChildrensCentresGroup = 2,
		Federation = 3,
		MultiacademyTrust = 4,
		SchoolSponsor = 5,
		SingleacademyTrust = 6,
		Trust = 7,
		UmbrellaTrust = 8,
    }

#endif
}
