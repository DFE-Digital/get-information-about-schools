
namespace Edubase.Services.Enums
{
#if (TEXAPI)
    public enum eLookupEstablishmentStatus
    {
        ARCHIVED = 2,
        Closed = 3,
        CreatedInError = 10,
        DeregisteredAsEYSetting = 6,
        Open = 1,
        OpenButProposedToClose = 4,
        PendingApproval = 8,
        ProposedToOpen = 5,
        Quarantine = 7,
        RejectedOpening = 9,
    }
#else
    public enum eLookupEstablishmentStatus
    {
        ARCHIVED = 1,
		Closed = 2,
		CreatedInError = 3,
		DeregisteredAsEYSetting = 4,
		Open = 5,
		OpenButProposedToClose = 6,
		PendingApproval = 7,
		ProposedToOpen = 8,
		Quarantine = 9,
		RejectedOpening = 10,
    }
#endif
}
