using System.ComponentModel.DataAnnotations;

namespace Edubase.Services.Enums
{
    public enum eProprietorType
    {
        [Display(Name = "Single Proprietor")]
        SingleProprietor = 1,

        [Display(Name = "Proprietor Body")]
        ProprietorBody = 2
    }
}
