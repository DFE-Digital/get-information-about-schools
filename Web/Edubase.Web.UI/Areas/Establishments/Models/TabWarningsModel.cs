using Edubase.Services.Enums;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class TabWarningsModel
    {
        private static readonly string AcademySecure16to19AddressWarning = "The address and location fields are representative of the care of address for safeguarding reasons.";

        public TabWarningsModel(int? establishmentTypeId)
        {
            if ( (establishmentTypeId != null) && (establishmentTypeId == (int) eLookupEstablishmentType.AcademySecure16to19) )
            {
                LocationTab = AcademySecure16to19AddressWarning;
                DetailsTab = AcademySecure16to19AddressWarning;
            }
            else
            {
                LocationTab = string.Empty;
                DetailsTab = string.Empty;
            }
        }

        public string DetailsTab { get; }
        public string LocationTab { get; }
    }
}
