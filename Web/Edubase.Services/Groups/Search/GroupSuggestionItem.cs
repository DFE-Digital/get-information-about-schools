namespace Edubase.Services.Groups.Search
{
    public class GroupSuggestionItem
    {
        public string Id => GroupUID.ToString();
        public int GroupUID { get; set; }
        public string Name { get; set; }
        public string Text => Name;
    }
}
