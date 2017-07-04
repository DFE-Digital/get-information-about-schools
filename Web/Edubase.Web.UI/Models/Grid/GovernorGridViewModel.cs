using Edubase.Services.Enums;
using Edubase.Services.Governors.Models;

namespace Edubase.Web.UI.Models.Grid
{
    public class GovernorGridViewModel : GridViewModel<GovernorModel>
    {
        public GovernorGridViewModel(string title) : base(title)
        { 
        }

        public eLookupGovernorRole Role { get; set; }
        public bool IsSharedRole { get; set; }
        public int? GroupUid { get; set; }
        public int? EstablishmentUrn { get; set; }
        public bool IsHistoricRole { get; set; }
    }
}