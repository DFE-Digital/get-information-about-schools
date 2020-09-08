using System.ComponentModel.DataAnnotations;

namespace Edubase.Services.Enums
{
    public enum eProprietorType
    {
        [Display(Name = "Individual Proprietor")]
        IndividualProprietor = 1,

        [Display(Name = "Proprietor Body")]
        ProprietorBody = 2,

        [Display(Name = "Not Applicable"), EnumGias(true)]
        NotApplicable = 3
    }
}
