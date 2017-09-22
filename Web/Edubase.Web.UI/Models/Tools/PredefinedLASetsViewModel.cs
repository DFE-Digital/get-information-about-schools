using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Web.UI.Models.Tools
{
    public class PredefinedLASetsViewModel
    {
        public int Skip { get; set; }
        public PaginatedResult<LocalAuthoritySet> Results { get; internal set; }
        public bool HasResults => Results != null && Results.Items != null & Results.Items.Any();
        public IEnumerable<LookupDto> LocalAuthorities { get; set; }
        public string GetLANames(int[] ids) => StringUtil.ConcatNonEmpties(", ", LocalAuthorities.Where(x => ids.Contains(x.Id)).OrderBy(x => x.Name).Select(x => x.Name).ToArray());
    }
}