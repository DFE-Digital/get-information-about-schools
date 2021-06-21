using System.ComponentModel.DataAnnotations;

namespace Edubase.Common
{
    public enum eNotificationBannerImportance
    {
        [Display(Name = "Important", Description = "Important (blue notification banner shown)")]
        Important,

        [Display(Name = "Very important", Description = "Very important (red notification banner shown)")]
        VeryImportant
    }
}
