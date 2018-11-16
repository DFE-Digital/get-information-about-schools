using Edubase.Common.Spatial;

namespace Edubase.Services.Geo
{
    public class PlaceDto
    {
        public string Name { get; set; }
        public LatLon Coords { get; set; }

        public PlaceDto()  { }

        public PlaceDto(string name, LatLon coords)
        {
            Name = name;
            Coords = coords;
        }
    }
}
