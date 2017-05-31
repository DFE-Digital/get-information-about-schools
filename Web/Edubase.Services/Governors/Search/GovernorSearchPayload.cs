using Edubase.Services.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Edubase.Services.Governors.Search
{
    public class GovernorSearchPayload
    {
        Dictionary<eGovernorTypesFlag, string> _govTypesMap => new Dictionary<eGovernorTypesFlag, string>
        {
            [eGovernorTypesFlag.MultiAcademyTrusts] = "mats",           // filtered to governors associated with MATs
            [eGovernorTypesFlag.AcademiesWithinMAT] = "acads_in_mat",   // filtered to governors of academies associated with an MAT
            [eGovernorTypesFlag.AcademiesWithinSAT] = "acads_in_sat",   // filtered to governors of academies associated with an SAT
            [eGovernorTypesFlag.GovsOfLAMaintained] = "la_maintained"   // filtered to governors of LA maintained type establishments
        };

        public GovernorSearchPayload()
        {

        }

        public GovernorSearchPayload(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        [JsonIgnore] // TODO: TEXCHANGE:  support Gid search
        public string Gid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int[] RoleIds { get; set; }
        public bool IncludeHistoric { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; } = 10;

        [JsonIgnore]
        public eGovernorTypesFlag? GovernorTypesFlag { get; set; }

        public string GovernorTypes => GovernorTypesFlag.HasValue ? _govTypesMap[GovernorTypesFlag.Value] : null;

        [JsonIgnore]
        public eSortBy SortBy { get; set; }

        [JsonProperty("sortBy")]
        internal string SortByDescriptor => SortBy.ToString();
    }
}
