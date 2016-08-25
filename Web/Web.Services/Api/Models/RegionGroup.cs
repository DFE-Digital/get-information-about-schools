using System.Collections.Generic;

namespace Web.Services.Api.Models
{
    public class RegionGroup
    {
        public static readonly IDictionary<string, string> _regionGroups = new Dictionary<string, string>
        {
            {"East Midlands", "14,16"},
            {"East of England", "25,27"},
            {"London Central", "31"},
            {"London East", "32"},
            {"London North", "33"},
            {"London South", "34"},
            {"London West", "35"},
            {"North East", "1,3"},
            {"North West", "6,7,9"},
            {"North Yorkshire and The Humber", "10"},
            {"South and West Yorkshire", "12"},
            {"South East", "36,37,38,39"},
            {"South West", "43,45"},
            {"West Midlands", "20,22,24"}
        };

        public static IDictionary<string, string> ListAll()
        {
            return _regionGroups;
        }
    }
}