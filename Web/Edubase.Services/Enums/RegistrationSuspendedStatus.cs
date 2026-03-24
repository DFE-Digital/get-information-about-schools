using System.ComponentModel.DataAnnotations;

namespace Edubase.Services.Enums
{
    public enum RegistrationSuspendedStatus
    {
        [Display(Name = "Education and boarding suspended")]
        EducationAndBoardingSuspended = 1,

        [Display(Name = "Education suspended")]
        EducationSuspended = 2,

        [Display(Name = "Not applicable")]
        NotApplicable = 3,

        [Display(Name = "Not recorded")]
        NotRecorded = 4
    }
}
