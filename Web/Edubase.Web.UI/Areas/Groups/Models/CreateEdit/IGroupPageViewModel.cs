namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public interface IGroupPageViewModel
    {
        int? GroupUId { get; set; }
        string ListOfEstablishmentsPluralName { get; set; }
        string GroupName { get; set; }
        int? GroupTypeId { get; set; }
        string GroupTypeName { get; set; }
        string Layout { get; set; }
        string SelectedTabName { get; set; }
    }
}
