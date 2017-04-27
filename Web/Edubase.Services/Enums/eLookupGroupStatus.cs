
namespace Edubase.Services.Enums
{
#if(TEXAPI)
    public enum eLookupGroupStatus
    {
        Open = 1,
        Closed = 2,
        ProposedToOpen = 3,
        ProposedToClose = 4,
        CreatedInError = 5,
    }
#else
    public enum eLookupGroupStatus
    {
        Closed = 1,
		CreatedInError = 2,
		Open = 3,
    }
#endif
}
