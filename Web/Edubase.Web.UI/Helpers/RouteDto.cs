using System.Linq;
using Microsoft.AspNetCore.Routing;

namespace Edubase.Web.UI.Helpers
{
    public class RouteDto
    {
        public string RouteName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
        public string Label { get; set; }

        public RouteDto(string routeName, RouteValueDictionary routeValues, string label)
        {
            RouteName = routeName;
            RouteValues = routeValues;
            Label = label;
        }

        public static RouteDto[] Create(params RouteDto[] dtos) => dtos.Where(x => x != null).ToArray();
    }
}
