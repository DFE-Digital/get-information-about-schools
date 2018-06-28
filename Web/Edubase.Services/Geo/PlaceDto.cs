namespace Edubase.Services.Geo
{
    public class PlaceDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public PlaceDto()  { }

        public PlaceDto(string id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}
