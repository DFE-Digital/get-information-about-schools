namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public interface IEstablishmentPageViewModel
    {
        string SelectedTab { get; set; }
        int? Urn { get; set; }
        string Name { get; set; }
        TabDisplayPolicy TabDisplayPolicy { get; set; }
        string Layout { get; set; }
    }
}