using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Edubase.Services.Establishments.Enums;
using Edubase.Data.DbContext;
using Edubase.Services.Exceptions;
using Edubase.Services.Lookup;

namespace Edubase.Services.Establishments
{
    public class LAESTABService : ILAESTABService
    {
        private ICachedLookupService _cachedLookupService;

        #region EstabNoRules

        private static readonly string[][] _estabNoRules = new string[][]
        {
            new[]{"","0 - Not applicable","1 - Nursery","2 - Primary","3 - Middle Deemed Primary","4 - Secondary","5 - Middle Deemed Secondary","6 - 16 Plus","7 - All Through","9 - Unknown"},
            new[]{"1 - Community School","NO RULE","1000-1099; 1800-1899","2000-3999; 5200-5299; 5900-5949","2000-3999; 5200-5299; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","NO RULE"},
            new[]{"2 - Voluntary Aided School","NO RULE","NO RULE","2000-3999; 5200-5299; 5900-5949","2000-3999; 5200-5299; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","NO RULE","4000-4899; 5400-5499; 5900-5949","NO RULE"},
            new[]{"3 - Voluntary Controlled School","NO RULE","NO RULE","2000-3999; 5200-5299; 5900-5949","2000-3999; 5200-5299; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","NO RULE","4000-4899; 5400-5499; 5900-5949","NO RULE"},
            new[]{"5 - Foundation School","NO RULE","NO RULE","2000-3999; 5200-5299; 5900-5949","2000-3999; 5200-5299; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","NO RULE","4000-4899; 5400-5499; 5900-5949","NO RULE"},
            new[]{"6 - City Technology College","6900-6904","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"7 - Community Special School","7000-7798; 5950-5999","7000-7798; 5950-5999","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"8 - Non-Maintained Special School","7000-7798; 5950-5999","7000-7798; 5950-5999","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"9 - Independent School Approved for SEN Pupils","6000-6899","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"10 - Other Independent Special School","6000-6899","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"11 - Other Independent School","6000-6899","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"12 - Foundation Special School","7000-7798; 5950-5999","7000-7798; 5950-5999","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"14 - Pupil Referral Unit","1100-1149","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"15 - LA Nursery School","NO RULE","1000-1099; 1800-1899","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"17 - European Schools","6000-6899","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"18 - Further Education","8000-8699","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"22 - EY Setting","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED"},
            new[]{"23 - Playing for Success Centres","0940-0949","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"24 - Secure Units","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED"},
            new[]{"25 - Offshore Schools","6000-6899","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"26 - Service Childrens Education","6000-6899","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"27 - Miscellaneous","9900-99254900-4999","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"28 - Academy Sponsor Led","6905-6924","6905-6924","2000-3999; 5200-5299; 5900-5949; 6905-6924","2000-3999; 5200-5299; 5900-5949;6905-6924","4000-4899; 5400-5499; 5900-5949;6905-6924","4000-4899; 5400-5499; 5900-5949;6905-6924","6905-6924","4000-4899; 5400-5499; 5900-5949;6905-6924","6905-6924"},
            new[]{"29 - Higher Education Institutions","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED"},
            new[]{"30 - Welsh Establishment","1000-1149; 1800-1899; 2000-4899; 5200-5299; 5400-5499; 5900-5949; 6000-6904; 7000-7798","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"31 - Sixth Form Centres","4900-4999","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"32 - Special Post 16 Institution","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"33 - Academy Special Sponsor Led","7000-7798; 5950-5999","7000-7798; 5950-5999","7000-7798; 5950-5999","7000-7798; 5950-5999","7000-7798; 5950-5999","7000-7798; 5950-5999","7000-7798; 5950-5999","7000-7798; 5950-5999","7000-7798; 5950-5999"},
            new[]{"34 - Academy Converter","NO RULE","1000-10991800-1899","2000-3999; 5200-5299; 5900-5949","2000-3999; 5200-5299; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","NO RULE"},
            new[]{"35 - Free Schools","NO RULE","NO RULE","2000-3999; 5200-5299; 5900-5949","2000-3999; 5200-5299; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","NO RULE","4000-4899; 5400-5499; 5900-5949","NO RULE"},
            new[]{"36 - Free Schools Special","7000-7798; 5950-5999","7000-77985950-5999","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"37 - British Schools Overseas","6000-6899","6000-6899","6000-6899","6000-6899","6000-6899","6000-6899","6000-6899","6000-6899","6000-6899"},
            new[]{"38 - Free Schools - Alternative Provision","1100-1149","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"39 - Free Schools - 16-19","NO RULE","NO RULE","NO RULE","NO RULE","4000-4899; 5400-5499","NO RULE","NO RULE","4000-4899; 5400-5499","NO RULE"},
            new[]{"40 - University Technical College","NO RULE","NO RULE","2000-3999; 5200-5299; 5900-5949","2000-3999; 5200-5299; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","NO RULE","4000-4899; 5400-5499; 5900-5949","NO RULE"},
            new[]{"41 - Studio Schools","NO RULE","NO RULE","2000-3999; 5200-5299; 5900-5949","2000-3999; 5200-5299; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","NO RULE","4000-4899; 5400-5499; 5900-5949","NO RULE"},
            new[]{"42 - Academy Alternative Provision Converter","1100-1149","1000-1099; 1800-1899","2000-3999; 5200-5299; 5900-5949","2000-3999; 5200-5299; 5900-5949","4000-4899; 5400-5499; 5900-5949","4000-4899; 5400-5499; 5900-5949","NO RULE","4000-4899; 5400-5499; 5900-5949","NO RULE"},
            new[]{"43 - Academy Alternative Provision Sponsor Led","1100-1149","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"44 - Academy Special Converter","5950-5999; 7000-7798","5950-5999; 7000-7798","5950-5999; 7000-7798","5950-5999; 7000-7798","5950-5999; 7000-7798","5950-5999; 7000-7798","5950-5999; 7000-7798","5950-5999; 7000-7798","5950-5999; 7000-7798"},
            new[]{"45 - Academy 16-19 Converter","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499"},
            new[]{"46 - Academy 16-19 Sponsor Led","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499","4000-4899; 4900-4999; 5400-5499"},
            new[]{"47 - Children's Centre","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED"},
            new[]{"48 - Children's Centre Linked Site","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED","NOT ALLOWED"},
            new[]{"56 - Institution funded by other Government Department","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"},
            new[]{"98 - Legacy types","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE","NO RULE"}
        };

        #endregion

        public LAESTABService(ICachedLookupService cachedLookupService)
        {
            _cachedLookupService = cachedLookupService;
        }

        public EstabNumberEntryPolicy GetEstabNumberEntryPolicy(int establishmentTypeId, int educationPhaseId)
        {
            string rule = FindRule(establishmentTypeId, educationPhaseId);
            return GetEstabNumberEntryPolicy(rule);
        }

        private static EstabNumberEntryPolicy GetEstabNumberEntryPolicy(string rule)
        {
            if (rule.Contains("NOT ALLOWED")) return EstabNumberEntryPolicy.NonePermitted;
            else if (rule.Contains("NO RULE")) return EstabNumberEntryPolicy.UserDefined;
            else return EstabNumberEntryPolicy.SystemGenerated;
        }

        public int GenerateEstablishmentNumber(int establishmentTypeId, int educationPhaseId, int localAuthorityId)
        {
            string rule = FindRule(establishmentTypeId, educationPhaseId);
            if (GetEstabNumberEntryPolicy(rule) != EstabNumberEntryPolicy.SystemGenerated) throw new Exception("The estab number should not be generated for this combination");
            var ranges = ParseRule(rule);
            using (var dc = new ApplicationDbContext())
            {
                foreach (var range in ranges)
                {
                    var maxAttempts = range.Item2 - range.Item1;
                    for (int i = 0; i < maxAttempts; i++)
                    {
                        var suggested = RandomNumber.Next(range.Item1, range.Item2);
                        var alreadyAllocated = dc.Establishments.Any(x => x.EstablishmentNumber == suggested && x.LocalAuthorityId == localAuthorityId);
                        if (!alreadyAllocated) return suggested;
                    }
                }
            }

            throw new DomainException($"All estab numbers have been allocated across all ranges for this rule: \"{rule}\"; input: establishmentTypeId:{establishmentTypeId}, "
                + $"educationPhaseId:{educationPhaseId}, localAuthorityId:{localAuthorityId}");
        }

        private string FindRule(int establishmentTypeId, int educationPhaseId)
        {
            var ruleColIndex = _estabNoRules.First().ToList().FindIndex(x => x.StartsWith(educationPhaseId + " "));
            var ruleRow = _estabNoRules.First(x => x.First().StartsWith(establishmentTypeId + " "));
            var rule = ruleRow[ruleColIndex];
            return rule;
        }

        private Tuple<int, int>[] ParseRule(string rule)
        {
            var retVal = new List<Tuple<int, int>>();
            var ranges = rule.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var range in ranges)
            {
                if (!range.IsNullOrEmpty())
                {
                    var parts = range.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToInteger()).Where(x => x.HasValue).Select(x => x.Value).ToArray();
                    if (parts.Length == 2 && parts[0] < parts[1])
                    {
                        retVal.Add(new Tuple<int, int>(parts[0], parts[1]));
                    }
                }
            }
            return retVal.ToArray();
        }

        public Dictionary<string, string> GetSimplifiedRules()
        {
            var retVal = new Dictionary<string, string>();
            var phases = _cachedLookupService.EducationPhasesGetAll().Select(x => int.Parse(x.Code));
            var types = _cachedLookupService.EstablishmentTypesGetAll().Select(x => int.Parse(x.Code));
            phases.ForEach(p => types.ForEach(t => retVal.Add(string.Concat(p, "-", t), GetEstabNumberEntryPolicy(t, p).ToString())));
            return retVal;
        }
    }
}
