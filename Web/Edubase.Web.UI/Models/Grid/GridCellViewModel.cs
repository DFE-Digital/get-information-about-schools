namespace Edubase.Web.UI.Models.Grid
{
    public class GridCellViewModel
    {
        public string Text { get; set; }
        public string SortKey { get; set; }
        public string SortType { get; set; }
        public GridCellViewModel Parent { get; set; }
        public GridCellViewModel(string text, string sortKey = "", string sortType = "")
        {
            Text = text;
            SortKey = sortKey;
            SortType = sortType;
        }
    }
}
