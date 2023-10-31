using Edubase.Services.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Services.Governors.Search
{
    public class GovernorSearchPayload
    {
        Dictionary<eGovernorTypesFlag, string> _govTypesMap => new Dictionary<eGovernorTypesFlag, string>
        {
            [eGovernorTypesFlag.MultiAcademyTrusts] = "mats",           // filtered to governors associated with MATs
            [eGovernorTypesFlag.AcademiesWithinMAT] = "acads_in_mat",   // filtered to governors of academies associated with an MAT
            [eGovernorTypesFlag.AcademiesWithinSAT] = "acads_in_sat",   // filtered to governors of academies associated with an SAT
            [eGovernorTypesFlag.GovsOfLAMaintained] = "la_maintained",   // filtered to governors of LA maintained type establishments
            [eGovernorTypesFlag.CTC] = "ctc",   // filtered to governors of City Technology Colleges
            [eGovernorTypesFlag.FreeSchools] = "freeschools",   // filtered to governors of Free Schools
            [eGovernorTypesFlag.AcadsWithSchoolSponsor] = "acads_with_sch_spon",   // filtered to governors of Academies with a School Sponsor
            [eGovernorTypesFlag.SecureSingleAcademyTrusts] = "ssats",   // filtered to governors associated with Secure SATs
        };

        public GovernorSearchPayload()
        {

        }

        public GovernorSearchPayload(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        [JsonProperty("governorId")]
        public string Gid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int[] RoleIds { get; set; }
        public bool IncludeHistoric { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; } = 10;

        [JsonIgnore]
        public eGovernorTypesFlag[] GovernorTypesFlags { get; set; }

        public string[] GovernorTypes => GovernorTypesFlags.Select(x => _govTypesMap[x]).ToArray();

        public int[] LocalAuthorityIds { get; set; }

        [JsonIgnore]
        public eSortBy SortBy { get; set; }

        [JsonProperty("sortBy")]
        internal string SortByDescriptor => SortBy.ToString();
    }
}
