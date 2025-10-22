using System.ComponentModel.DataAnnotations;
using Edubase.Common;
using Edubase.Services.Groups.Models;

namespace Edubase.Web.UI.Areas.Groups.Models
{
    public class ConvertSATViewModel
    {
        [Required(ErrorMessage = "Please enter Companies House number, Group UID or ID")]
        public string Text { get; set; }
        public SearchGroupDocument Details { get; internal set; }
        public string ActionName { get; set; } // find / confirm
        public string Token { get; set; }

        public string CountryName { get; set; }
        public string CountyName { get; set; }

        public bool CopyGovernanceInfo { get; set; }

        public string GetAddress() => StringUtil.ConcatNonEmpties(", ",
            Details.Address.Line1,
            Details.Address.Line2,
            Details.Address.Line3,
            Details.Address.CityOrTown,
            CountyName.Replace("Not recorded", string.Empty),
            Details.Address.PostCode,
            CountryName.Replace("Not recorded", string.Empty));
    }
}
