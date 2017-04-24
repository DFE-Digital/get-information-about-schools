using System.ComponentModel.DataAnnotations;
using Edubase.Common;

namespace Edubase.Web.UI.Models.Search
{
    public class GovernorSearchPayloadViewModel
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public int? RoleId { get; set; }
        public bool IncludeHistoric { get; set; }

        [Display(Name = "GID")]
        public int? Gid { get; set; }
    }
}