namespace Edubase.Services.Establishments.Search
{
    public class EstablishmentSuggestionItem
    {
        public int Id => Urn;
        public int Urn { get; set; }
        public string Name { get; set; }
        public string Address_CityOrTown { get; set; }
        public string Address_PostCode { get; set; }
        public string Text => $"{Name} ({Address_CityOrTown}, {Address_PostCode})";
    }
}
