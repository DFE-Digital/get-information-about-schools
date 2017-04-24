using System.ComponentModel.DataAnnotations;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class EditGroupDelegationInformation : IGroupPageViewModel
    {
        public int GroupId { get; set; }

        [Display(Name="Please provide details of the level of the local governing body's (LGB's) delegated authority, including a shared LGB if applicable"), MaxLength(1000)]
        public string DelegationInformation { get; set; }

        public int? GroupUId { get; set; }
        public string ListOfEstablishmentsPluralName { get; set; }
        public string GroupName { get; set; }
        public int? GroupTypeId { get; set; }
        public string Layout { get; set; }
        public string SelectedTabName { get; set; }
    }
}