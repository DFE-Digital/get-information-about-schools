namespace Edubase.Services.Domain
{
    public class AutocompleteItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public AutocompleteItemDto()
        {

        }

        public AutocompleteItemDto(string id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}
