using System.ComponentModel.DataAnnotations;

namespace Edubase.Common
{
    public enum eNotificationBannerImportance
    {
        [Display(Name = "Important")]
        Important,

        [Display(Name = "Very important")]
        VeryImportant
    }
}
