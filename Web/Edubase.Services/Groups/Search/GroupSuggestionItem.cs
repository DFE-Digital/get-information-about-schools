namespace Edubase.Services.Groups.Search
{
    public class GroupSuggestionItem
    {
        public string Id => GroupUId.ToString();
        public int GroupUId { get; set; }
        public string Name { get; set; }
        public string Text => Name;
    }
}
