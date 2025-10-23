using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models.Search
{
    public class GovernorSearchPayloadViewModel
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public int[] RoleId { get; set; } = new int[0];
        public bool IncludeHistoric { get; set; }

        [Display(Name = "GID")]
        public int? Gid { get; set; }
    }
}
