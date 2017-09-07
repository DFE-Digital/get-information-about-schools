using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class ValidateEstablishmentJoinedDateModel
    {
        public DateTimeViewModel JoinDate { get; set; }
        public DateTimeViewModel GroupOpenDate { get; set; }
        public string GroupType { get; set; }
    }
}