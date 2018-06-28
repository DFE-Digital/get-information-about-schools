namespace Edubase.Services.IntegrationEndPoints.OSPlaces.Models
{
    public class OSPlacesItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public OSPlacesItemDto()
        {

        }

        public OSPlacesItemDto(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
