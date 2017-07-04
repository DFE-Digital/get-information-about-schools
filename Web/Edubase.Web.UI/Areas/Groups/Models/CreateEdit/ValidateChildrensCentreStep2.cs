using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class ValidateChildrensCentreStep2
    {
        public string GroupName { get; set; }
        public DateTimeViewModel OpenDate { get; set; }
        public int? LocalAuthorityId { get; set; }
    }
}